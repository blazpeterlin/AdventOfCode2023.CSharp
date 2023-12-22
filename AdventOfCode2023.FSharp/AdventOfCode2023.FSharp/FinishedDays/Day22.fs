module Day22

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked

let (.+.) (x,y,z) (dx,dy,dz) = (x+dx,y+dy,z+dz)
let (.-.) (x,y,z) (dx,dy,dz) = (x-dx,y-dy,z-dz)
let (.*.) (x,y,z) multi = (x*multi,y*multi,z*multi)
let (./.) (x,y,z) div = (x/div,y/div,z/div)



let len (x,y,z) = abs(x)+abs(y)+abs(z)
type Pos = { X: int; Y: int; Z: int; }
type Brick = { A: Pos; B: Pos; Id:int; }

let p2tpl pos = (pos.X, pos.Y, pos.Z)

let allPos (brick:Brick) =
    let d = p2tpl(brick.B) .-. p2tpl(brick.A)

    if d = (0,0,0) then [p2tpl(brick.A)] else

    let num = len(d)
    let step = d ./. num
    seq { 0 .. num }
    |> Seq.map(fun i -> 
        let diff = step .*. i
        diff .+. p2tpl(brick.A)
    )
    |> List.ofSeq

let directUpwards brick = 
    if brick.B.Z < brick.A.Z 
    then { brick with A=brick.B; B= brick.A; }
    else brick

let parseBricks inputPath =
    
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let splitters = ",~".ToCharArray()

    let bricks =
        lns 
        |> List.map(fun ln -> 
            ln.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) 
            |> Seq.map int 
            |> List.ofSeq 
            |> function 
                | [x1;y1;z1;x2;y2;z2] -> { A={X=x1;Y=y1;Z=z1;}; B={X=x2;Y=y2;Z=z2;}; Id=0; }
                | _ -> failwith "huh"
        )
        |> List.map directUpwards
        |> List.indexed
        |> List.map(fun (idx,br) -> { br with Id=idx })

    bricks

let getBrickDependencies bricks =
    let mapBrickPos0 : Map<int*int, int*int> = Map.empty
    
    let (mapN,dependenciesN) = 
        bricks
        |> List.sortBy (fun brick -> min brick.A.Z brick.B.Z)
        |> List.fold(fun (mp, prevDependencies) brick -> 
            let brickPos = allPos brick 
            let fallingPosMinZ = brickPos |> List.groupBy (fun (x,y,z) -> (x,y)) |> List.map(fun (_,vals) -> vals |> List.minBy(fun (x,y,z) -> z))
            
            let bricksDeltaHeights = 
                fallingPosMinZ 
                |> List.map(fun (x,y,z) -> 
                    match Map.tryFind (x,y) mp with
                    | None -> (None,z-1)
                    | Some((brickIdx,zHeight)) -> (Some(brickIdx),z-zHeight-1)
                )
            let minDeltaZ = bricksDeltaHeights |> List.map snd |> List.min

            if minDeltaZ < 0 then failwith "huh" else

            let dependencies = bricksDeltaHeights |> List.filter(fun(_,deltaZ) -> deltaZ=minDeltaZ) |> List.choose fst |> List.distinct |> List.map(fun depOn -> (depOn, brick.Id))
            let nextMap =
                brickPos
                |> List.fold(fun (mpi:Map<int*int, int*int>) pos ->
                    let felledPos = pos .-. ((0,0,1) .*. minDeltaZ)
                    let (fx,fy,fz) = felledPos
                    let key = (fx,fy)
                    mpi.Add (key, (brick.Id, fz))
                ) mp

            (nextMap, dependencies@prevDependencies)
        ) (mapBrickPos0, [])

    dependenciesN


let solve1 inputPath = 
    let bricks = parseBricks inputPath

    let dependenciesN = getBrickDependencies bricks
    
    let bricksTooDependent = dependenciesN |> List.countBy snd |> List.filter(fun (_,count) -> count=1) |> List.map fst |> Set
    let tooDependentOn = dependenciesN |> List.filter(fun (depOn,depBy) -> bricksTooDependent.Contains(depBy)) |> List.map fst |> List.distinct
    let res = bricks |> List.map(_.Id) |> List.except tooDependentOn |> List.length

    res

let solve2 inputPath =
    let bricks = parseBricks inputPath

    let dependenciesN = getBrickDependencies bricks
    let dependenciesByAffectedBrick = dependenciesN |> List.groupBy snd |> List.map(fun (s,lst) -> (s,(lst|>List.map fst)))
    
    let bricksTooDependent = dependenciesN |> List.countBy snd |> List.filter(fun (_,count) -> count=1) |> List.map fst |> Set
    let dangerousDependencies = dependenciesN |> List.filter(fun (depOn,depBy) -> bricksTooDependent.Contains(depBy))

    let dangerouslyDependentOn = dangerousDependencies |> List.map fst |> List.distinct
    let dangerousConsequences = 
        dangerouslyDependentOn
        |> List.map (fun d ->
            let finalConsequences = 
                [d] 
                |> Set
                |> Seq.unfold(fun crushedBricks -> 
                    let allAffected = 
                        dependenciesByAffectedBrick
                        |> List.filter (fun (brickId, depOn) ->
                            depOn |> List.forall(fun d -> crushedBricks.Contains(d))
                        )
                        |> List.map fst

                    let nextCrushedBricks = allAffected |> List.fold (fun (cb:Set<int>) a -> cb.Add(a)) crushedBricks
                    if nextCrushedBricks = crushedBricks 
                    then None
                    else Some(nextCrushedBricks, nextCrushedBricks)
                )
                |> Seq.last
                |> Set.count
            finalConsequences-1
        )
        

    let res = dangerousConsequences |> List.sum

    res