
module Day16

open System.Collections.Generic

let (+..) (x1,y1) (x2,y2) = (x1+x2,y1+y2)

type Dir = LEFT | RIGHT | UP | DOWN
let dir2tpl dir = match dir with | LEFT -> (-1,0) | RIGHT -> (1,0) | UP -> (0,-1) | DOWN -> (0,1)
type Beam = { pos: int*int; dir: Dir; }

let getNextDirs dir mapChar =
    let res : Dir list = 
        match (mapChar, dir) with
        | ('.',_) -> [dir]

        | ('|',UP) -> [dir]
        | ('|',LEFT) -> [UP;DOWN]
        | ('-',LEFT) -> [dir]
        | ('-',UP) -> [LEFT;RIGHT]
        | ('|',DOWN) -> [dir]
        | ('|',RIGHT) -> [UP;DOWN]
        | ('-',RIGHT) -> [dir]
        | ('-',DOWN) -> [LEFT;RIGHT]

        | ('/',LEFT) -> [DOWN]
        | ('/',RIGHT) -> [UP]
        | ('/',UP) -> [RIGHT]
        | ('/',DOWN) -> [LEFT]

        | ('\\',LEFT) -> [UP]
        | ('\\',RIGHT) -> [DOWN]
        | ('\\',UP) -> [LEFT]
        | ('\\',DOWN) -> [RIGHT]

        | _ -> failwith "huh"
    res

let nextState (coordMap:Map<int*int,char>) state =
    let (beams,visited,beamsVisited:Set<Beam>) = state
    let beamsFiltered = beams |> Seq.filter(fun (beam: Beam) -> Map.containsKey beam.pos coordMap)
    let beamsNext =
      beamsFiltered
      |> Seq.map(fun (beam:Beam) ->
        let mapChar = coordMap[beam.pos]
        let nextDirs = getNextDirs beam.dir mapChar

        nextDirs |> Seq.map(fun (nextDir:Dir) -> { pos= dir2tpl(nextDir) +.. beam.pos; dir=nextDir })
      )
      |> Seq.concat
      |> Seq.filter(fun beamNext -> not (beamsVisited.Contains(beamNext)))
      |> Seq.distinct
      |> List.ofSeq

    let visitedAdd = beamsFiltered |> Seq.map(fun beam -> beam.pos) |> List.ofSeq
    let visitedNext = visitedAdd @ visited
    let beamsVisitedNext =
      beamsNext
      |> Seq.fold (fun (mapset:Set<Beam>) (beam:Beam) -> mapset.Add(beam)) beamsVisited

    let r = (beamsNext, visitedNext, beamsVisitedNext)
    Some (r,r)

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath);
    let coordMap = 
        lns 
        |> Seq.indexed 
        |> Seq.map(fun (y,ln) -> 
            ln.ToCharArray() 
            |> Seq.indexed 
            |> Seq.map(fun (x,ch) -> ((x,y),ch) )
        )
        |> Seq.concat
        |> Map.ofSeq
    
    let beam0 = { pos=(0,0); dir=RIGHT }
    let state0 = ([beam0], [(0,0)], Set.empty)

    let stateN = 
        state0
        |> Seq.unfold(fun st -> nextState coordMap st)
        |> Seq.find(fun (liveStates,_,_) -> Seq.isEmpty(liveStates))

    let (_,energized,_) = stateN
    let res = energized |> Seq.distinct |> Seq.length

    res

let solve2 inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath);
    let coordMap = 
        lns 
        |> Seq.indexed 
        |> Seq.map(fun (y,ln) -> 
            ln.ToCharArray() 
            |> Seq.indexed 
            |> Seq.map(fun (x,ch) -> ((x,y),ch) )
        )
        |> Seq.concat
        |> Map.ofSeq

    let maxX = coordMap.Keys |> Seq.map(fun (x,_) -> x) |> Seq.max
    let maxY = coordMap.Keys |> Seq.map(fun (_,y) -> y) |> Seq.max
    
    let verticalCandidates = [0..maxX] |> Seq.map(fun x -> [{ pos=(x,0); dir=DOWN }; { pos=(x,maxY); dir=UP }]) |> List.concat
    let horizontalCandidates = [0..maxY] |> Seq.map(fun y -> [{ pos=(0,y); dir=RIGHT }; { pos=(maxX,y); dir=LEFT }]) |> List.concat
    let candidateBeams = verticalCandidates@horizontalCandidates

    let res =
        candidateBeams
        |> Seq.map(fun candidateBeam -> 
        
            let state0 = ([candidateBeam], [candidateBeam.pos], Set.empty)
            
            state0
            |> Seq.unfold(fun st -> nextState coordMap st)
            |> Seq.find(fun (liveStates,_,_) -> Seq.isEmpty(liveStates))
        )
        |> Seq.map (fun (_,energized,_) -> Seq.distinct energized |> Seq.length)
        |> Seq.max

    // not 8184 too low
    res