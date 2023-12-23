module Day23

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked

let (.+.) (x,y) (dx,dy) = (x+dx,y+dy)

let allDirs = [ 1,0 ; -1,0 ; 0,-1 ; 0,1 ]

type State1 = { pos: int*int; traversed: (int*int) Set; stepsTaken: int; }
type State2 = { pos: int*int; reachable: (int*int) Set; }

let dirMatcherPart1 (mp:Map<int*int,char>) pos = 
    let possibleNextDirs = 
        match mp[pos] with
        | '.' -> allDirs
        | '>' -> [ 1,0 ]
        | '<' -> [ -1,0 ]
        | '^' -> [ 0,-1 ]
        | 'v' -> [ 0,1 ]
        | _ -> failwith "huh"
    possibleNextDirs

let getNextStates1 (mp:Map<int*int,char>) dirMatcher (st:State1) : State1 list =

    let possibleNextDirs = dirMatcher st.pos
    let nextPos = 
        possibleNextDirs 
        |> List.map(fun dir -> st.pos .+. dir) 
        |> List.filter(fun p -> not(st.traversed.Contains(p)) && mp.ContainsKey(p) && mp[p] <> '#')

    if nextPos.Length=0 then [] else

    let nextStates = 
        nextPos
        |> List.map(fun p ->
            { pos=p; traversed=st.traversed.Add(p); stepsTaken=st.stepsTaken+1; }
        )

    nextStates

let findMaxSteps (mp:Map<int*int,char>) dirMatcher (sts:State1 list) =
    let stN = 
        sts
        |> Seq.unfold (fun ists -> 
            let r = ists |> List.map(getNextStates1 mp dirMatcher) |> List.concat

            if r.IsEmpty then None else Some(r,r)
        )
        |> Seq.last
        
    stN[0].stepsTaken

let findAllReachables (p:int*int) (reachable:(int*int) Set) =
    let (rReachable, _) = 
        ([p] |> Set, [p])
        |> Seq.unfold (fun (accSet, edges) ->
            if edges=[] then None else
            let nextEdges = 
                Seq.allPairs edges allDirs
                |> Seq.map (fun (a,b) -> a.+.b)
                |> Seq.filter(fun p -> reachable.Contains(p) && not(accSet.Contains(p)))
                |> Seq.distinct
                |> List.ofSeq
            let nextAcc = nextEdges |> List.fold (fun (s:Set<int*int>) e -> s.Add(e)) accSet
            let r = (nextAcc, nextEdges)
            Some(r,r)
        )
        |> Seq.last
    rReachable

let parseMapList inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let lst = lns |> Seq.indexed |> Seq.map(fun (y,ln) -> ln.ToCharArray() |> Seq.indexed |> Seq.map(fun (x,ch) -> ((x+1,y+1),ch))) |> Seq.concat |> Seq.toList
    lst

let solve1 inputPath = 
    let lst = parseMapList inputPath
    let mp = lst |> Map.ofSeq

    let pos0 = lst |> Seq.find(fun ((x,y),ch) -> ch='.' && y=1) |> fst
    let st0 = [{ pos=pos0; traversed=[pos0]|>set; stepsTaken=0; }]

    let res = findMaxSteps mp (dirMatcherPart1 mp) st0
    res

let getIntersections (mp:Map<int*int, char>) =
    mp
    |> Seq.filter(fun kvp -> kvp.Value <> '#')
    |> Seq.map (_.Key)
    |> Seq.filter(fun k -> 
        let neighbours = allDirs |> List.map ((.+.)k) |> List.filter(fun p -> mp.ContainsKey(p) && mp[p] <> '#')
        neighbours.Length>=3 || neighbours.Length=1
    )
    |> List.ofSeq

let getDistIfNear (reachableInput:Set<int*int>) allPts pt1 pt2 =
    if pt1=pt2 then None else
    let reachableWithoutOtherPoints = reachableInput |> Seq.except allPts |> Set |> Set.add pt1 |> Set.add pt2
    // find shortest path length, or none when there is another Vertex inbetween
    let lastAttempt =
        ([pt1] |> Set, [pt1], 0)
        |> Seq.unfold (fun (reachedPtsSet, lastSpawned, steps) -> 
            if reachedPtsSet.Contains(pt2) then None else
            let nextSpawned = 
                Seq.allPairs lastSpawned allDirs 
                |> Seq.map (fun (a,b) -> a .+. b)
                |> Seq.filter(reachableWithoutOtherPoints.Contains)
                |> Seq.filter(not << reachedPtsSet.Contains)
                |> List.ofSeq
            let nextSet =
                nextSpawned
                |> Seq.fold (fun (s:Set<int*int>) pt -> s.Add(pt)) reachedPtsSet
            if nextSet = reachedPtsSet then None else
            let r = (nextSet, nextSpawned, steps+1)
            Some(r,r)
        )
        |> Seq.tryLast
    if lastAttempt=None then None else
    let (lastSet, _, numSteps) = lastAttempt.Value
    if lastSet.Contains(pt2) then Some(numSteps) else None

let rec longestPathDist (mapE:Map<(int*int)*(int*int),int>) (goal:int*int) (pt:int*int) (traversed:(int*int) Set) =
    if pt = goal then Some(0) else
    let nextEs = mapE |> Map.toSeq |> Seq.filter(fun ((a,b),v) -> a=pt) |> Seq.map (fun ((vFrom,vTo),dist) -> vTo,dist) |> Seq.filter(fun (pt,_) -> not(traversed.Contains(pt))) |> List.ofSeq
    if nextEs.IsEmpty then None else
    let longestPaths = 
        nextEs
        |> List.choose(fun (v,edgeCost) ->
            let innerDist = longestPathDist mapE goal v (traversed.Add v)
            if innerDist=None 
            then None 
            else Some(innerDist.Value + edgeCost)
        )
    if longestPaths.IsEmpty then None else
    Some(longestPaths |> List.max)

let solve2 inputPath =
    let lst = parseMapList inputPath
    let mp:Map<int*int,char> = lst |> Map.ofSeq
    
    let reachable = mp |> Seq.filter(fun kvp -> kvp.Value<>'#') |> Seq.map (fun kvp -> kvp.Key) |> Set
    let graphV = getIntersections mp
    let graphE = 
        List.allPairs graphV graphV
        |> List.choose(fun (a,b) -> 
            let dist = getDistIfNear reachable graphV a b
            if dist=None 
            then None
            else Some((a,b),dist.Value)
        )
        |> Map

    let maxY = mp.Keys |> Seq.map snd |> Seq.max
    let start = lst |> Seq.find(fun ((x,y),ch) -> ch<>'#' && y=1) |> fst
    let goal = lst |> Seq.find(fun ((x,y),ch) -> ch<>'#' && y=maxY) |> fst

    let traversed = [start] |> Set

    let res = longestPathDist graphE goal start traversed


    res