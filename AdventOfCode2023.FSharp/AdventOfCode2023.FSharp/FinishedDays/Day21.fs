module Day21

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked

let dirs = [(0,-1);(0,1);(-1,0);(1,0)]
let dirsAndSelf = [(0,-1);(0,1);(-1,0);(1,0); 0,0]

let (.+.) (x,y) (dx,dy) = (x+dx,y+dy)

let step (map:Map<int*int,char>) (s:(int*int) list) = 
    Seq.allPairs s dirs
    |> Seq.map (fun (pos,dir) -> pos .+. dir)
    |> Seq.distinct
    |> Seq.filter(fun p -> map.ContainsKey p && map[p] = '.')
    |> List.ofSeq
    
let stepStay (map:Map<int*int,char>) (s:(int*int) Set) = 
    Seq.allPairs s dirsAndSelf
    |> Seq.map (fun (pos,dir) -> pos .+. dir)
    |> Seq.distinct
    |> Seq.filter(fun p -> map.ContainsKey p && map[p] = '.')
    |> Set.ofSeq

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let map = lns |> Seq.indexed |> Seq.map(fun (y,ln) -> ln.ToCharArray() |> Seq.indexed |> Seq.map(fun (x,ch) -> ((x,y),ch))) |> Seq.concat |> Map
    let (posS,_) = map |> Map.toSeq |> Seq.find(fun (k,v) -> v = 'S')
    let map0 = map.Add(posS,'.')

    let st0 = [posS]

    let stN = 
        seq { 1 .. 64 }
        |> Seq.fold (fun st i -> step map0 st) st0

    let res = stN |> List.length


    res

//let step2 (xmod, ymod) (map:Map<int*int,char>) (s:(int*int) list) = 
//    Seq.allPairs s dirs
//    |> Seq.map (fun (pos,dir) -> pos .+. dir)
//    |> Seq.distinct
//    |> Seq.filter(fun (x,y) -> map[(x%xmod + xmod) % xmod,(y%ymod + ymod) % ymod] = '.')
//    |> List.ofSeq
    
let rec gcd x y =
    if y = 0 then x
    else gcd y (x % y)
    
let lcm a b = a*b/(gcd a b)

//let timeToPos (pos:int*int) (xmod, ymod) (map:Map<int*int,char>) (s:(int*int) list) = 
//    Seq.initInfinite id
//    |> Seq.scan (fun st i -> step2 (xmod,ymod) map st) s
//    |> Seq.indexed
//    |> Seq.find(fun (i,lst) -> List.contains pos lst)
//    |> fst

let timeToFill (map:Map<int*int,char>) (s:(int*int) list) = 
    Seq.initInfinite id
    |> Seq.scan (fun st i -> stepStay map st) (Set s)
    |> Seq.indexed
    |> Seq.pairwise
    |> Seq.find(fun ((_,lst1),(_,lst2)) -> (Set.count lst1)=(Set.count lst2))
    |> fst
    |> fst

let sizeToFill (map:Map<int*int,char>) (s:(int*int) list) time = 
    seq { 1 .. time }
    |> Seq.scan (fun st i -> stepStay map st) (Set s)
    |> Seq.indexed
    |> Seq.pairwise
    |> Seq.find(fun ((_,lst1),(_,lst2)) -> (Set.count lst1)=(Set.count lst2))
    |> fst
    |> fun (_,s) -> s.Count
    
let sizeFilled (map:Map<int*int,char>) (s:(int*int) list) time = 
    seq { 1 .. time }
    |> Seq.fold (fun st i -> step map st) (s)
    |> List.length

let solve2 n inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let map = lns |> Seq.indexed |> Seq.map(fun (y,ln) -> ln.ToCharArray() |> Seq.indexed |> Seq.map(fun (x,ch) -> ((x,y),ch))) |> Seq.concat |> Map
    let (posS,_) = map |> Map.toSeq |> Seq.find(fun (k,v) -> v = 'S')
    let map0 = map.Add(posS,'.')

    let st0 = [posS]

    let maxX = map0.Keys |> Seq.map(fun (x,y) -> x) |> Seq.max
    let maxY = map0.Keys |> Seq.map(fun (x,y) -> y) |> Seq.max

    let len = lcm (maxX+1) (maxY+1)

    let time2firstMapBreach = len/2+1

    let centerEdgeBreaches = [ 0, len/2 ; len/2, 0; len-1, len/2; len/2,len-1 ]

    // len 131

    //let n = 26501365
    let remainingAfterFirstBreach = n - time2firstMapBreach
    let remainingAfterLastBreach = remainingAfterFirstBreach % (len)

    // diamond shape

    let mapSizeOdd = sizeFilled map0 st0 151 |> int64
    let mapSizeEven = sizeFilled map0 st0 152 |> int64

    // dist (in maps) from center (incl.) to edge (incl.)
    let r = int64((n-time2firstMapBreach) / len) + 2L
    
    let filledFromCenter =  (n-len/2-len/2)/len + 1

    let numAllFilledMaps = seq { 0 .. (filledFromCenter-1) } |> Seq.map (fun x -> if x=0 then 1 else 4*x) |> Seq.map int64 |> Seq.sum
    let numMainMaps = seq { 0 .. 2 .. (filledFromCenter-1) } |> Seq.map (fun x -> if x=0 then 1 else 4*x) |> Seq.map int64 |> Seq.sum

    let mainIsEven = n%2=0

    let (numEvenFilledMaps, numOddFilledMaps) = 
        (
            if mainIsEven then numMainMaps else (numAllFilledMaps - numMainMaps)
            , if mainIsEven then (numAllFilledMaps - numMainMaps) else numMainMaps
        )
    
    let fullyFilledPart = numOddFilledMaps * mapSizeOdd + numEvenFilledMaps * mapSizeEven
    
    let numMapsForEachDiagonalTotal = r-2L + int64(remainingAfterLastBreach / (len/2+1))
    let numStepsForLastDiagonal = (remainingAfterLastBreach - (len/2) - 1 + len) % len

    let numMapsForEachDiagonal1 = numMapsForEachDiagonalTotal
    let numMapsForEachDiagonal2 = if remainingAfterLastBreach > len/2 then numMapsForEachDiagonalTotal-1L else 0L
    //let numMapsForEachDiagonal2 = numMapsForEachDiagonalTotal / 2L
    //let numMapsForEachDiagonal1 = numMapsForEachDiagonalTotal - numMapsForEachDiagonal2
    
    let diagonalBreaches = [ 0,0 ; len-1,0 ; 0,len-1 ; len-1,len-1 ]
    let lastDiagonalFilledSizes1 = diagonalBreaches |> Seq.map (fun pt -> sizeFilled map0 [pt] numStepsForLastDiagonal) |> List.ofSeq
    let lastDiagonalFilledSizes2 = diagonalBreaches |> Seq.map (fun pt -> sizeFilled map0 [pt] (numStepsForLastDiagonal+len)) |> List.ofSeq
    let lastDiagonalsFilledPart = 
        (lastDiagonalFilledSizes1 |> Seq.map(fun s -> int64(s) * numMapsForEachDiagonal1) |> Seq.sum)
        + (lastDiagonalFilledSizes2 |> Seq.map(fun s -> int64(s) * numMapsForEachDiagonal2) |> Seq.sum)

    let numStepsForLastEdge = remainingAfterLastBreach
    let lastFourEdgesFilledSizes = (centerEdgeBreaches |> Seq.map (fun pt -> sizeFilled map0 [pt] numStepsForLastEdge)) |> List.ofSeq
    let lastFourEdgesFilledPart = lastFourEdgesFilledSizes |> Seq.sum |> int64

    let res = fullyFilledPart + lastDiagonalsFilledPart + lastFourEdgesFilledPart

    // attempts log

    // 1251175692953489 too high
    // 1251163323529913 too high
    // 1251150954152233 too high
    // 1251163323560778 .. oh not the area
    // 627464189717583 nope
    // 623699174707827 nope
    // 622287224285151 nope
    // 629346789491699 nope
    // 625581700824259 nope
    // 625587097176759 nope
    // 625587097150084 

    res