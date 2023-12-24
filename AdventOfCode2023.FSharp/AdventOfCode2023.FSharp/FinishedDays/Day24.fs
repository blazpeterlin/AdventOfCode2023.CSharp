module Day24

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked
open Microsoft.Z3.Addons
open Microsoft.Z3

type Coord = { X:float; Y:float; Z:float;}
type Hailstone = { Pos:Coord; Vel: Coord; }

type CoordInt64 = { isInt64:bool; X:int64; Y:int64; Z:int64;}
type HailstoneInt64 = { isInt64:bool; Pos:CoordInt64; Vel: CoordInt64; }

//open Microsoft.Z3.Int
open Microsoft.Z3.Real

let slope2d (c:Coord) = c.Y / c.X

let collision2d (h1:Hailstone) (h2:Hailstone) =
    let slope1 = slope2d (h1.Vel)
    let slope2 = slope2d (h2.Vel)

    if slope1 = slope2 then None else

    let commonX = (slope2 * h2.Pos.X - slope1 * h1.Pos.X + h1.Pos.Y - h2.Pos.Y) / (slope2 - slope1)
    let commonY = slope1 * (commonX - h1.Pos.X) + h1.Pos.Y

    Some({ X = commonX; Y=commonY; Z=0; })


let isInFuture (h:Hailstone) (v:Coord) =
    (h.Vel.X >= 0 && v.X >= h.Pos.X || h.Vel.X <= 0 && v.X <= h.Pos.X)
    && (h.Vel.Y >= 0 && v.Y >= h.Pos.Y || h.Vel.Y <= 0 && v.Y <= h.Pos.Y)

let parseHailstonesFloat inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let splitters = " ,@".ToCharArray()
    let hailstones = lns |> Seq.map(fun ln -> 
        ln.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) 
        |> List.ofArray 
        |> List.map (float)
        |> fun spl -> match spl with | [x;y;z;dx;dy;dz] -> { Pos={X=x;Y=y;Z=z;}; Vel={X=dx;Y=dy;Z=dz;} } | _ -> failwith "huh"
    )
    hailstones

let solve1 inputPath = 
    let hailstones = parseHailstonesFloat inputPath

    let minDim=float 200000000000000L
    let maxDim=float 400000000000000L
    
    //let minDim=float 7
    //let maxDim=float 27

    let intersectionsCount = 
        Seq.allPairs hailstones hailstones
        |> Seq.filter(fun (h1,h2) -> h1 <> h2)
        |> Seq.distinctBy(fun (h1,h2) -> [h1;h2]|>Set)
        |> Seq.map (fun (h1,h2) -> (h1,h2,collision2d h1 h2))
        |> Seq.filter(fun (_,_,opt) -> opt.IsSome)
        |> Seq.filter(fun ((h1:Hailstone),(h2:Hailstone),intersOpt) -> 
            let inters = intersOpt.Value
            let isWithinBoundary = inters.X >= minDim && inters.X <= maxDim && inters.Y >= minDim && inters.Y <= maxDim
            let isInH1Future = isInFuture h1 inters
            let isInH2Future = isInFuture h2 inters
            isWithinBoundary && isInH1Future && isInH2Future
        )
        |> Seq.length

    intersectionsCount

let parseHailstonesInt64 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let splitters = " ,@".ToCharArray()
    let hailstones = lns |> Seq.map(fun ln -> 
        ln.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) 
        |> List.ofArray 
        |> List.map (int64)
        |> fun spl -> match spl with | [x;y;z;dx;dy;dz] -> { isInt64=true; Pos={ isInt64=true; X=x;Y=y;Z=z;}; Vel={ isInt64=true;X=dx;Y=dy;Z=dz;} } | _ -> failwith "huh"
    )
    hailstones


let solve2 inputPath =
    let hailstones = parseHailstonesFloat inputPath

    // free variables: res.x .y .z .dx .dy .dz
    // additional free variables: t1 t2 .... t300
    // locked variables: for i in 1 ... 300:  hi .x .y .z .dx .dy .dz

    // equations:
    // res.x + t0 * res.dx = h0.x + t0 * h.dx
    // res.y + t0 * res.dy = h0.y + t0 * h.dy
    // res.z + t0 * res.dz = h0.z + t0 * h.dz

    // find free variables under res

    let opt = Opt()

    let x = Real "x"
    let y = Real "y"
    let z = Real "z"
    let dx = Real "dx"
    let dy = Real "dy"
    let dz = Real "dz"

    for (idx,h) in (hailstones |> Seq.indexed |> Seq.truncate 3) do
        let h_x = h.Pos.X |> RealVal
        let h_y = h.Pos.Y |> RealVal
        let h_z = h.Pos.Z |> RealVal
        let h_dx = h.Vel.X |> RealVal
        let h_dy = h.Vel.Y |> RealVal
        let h_dz = h.Vel.Z |> RealVal
        let t = ("t_" + idx.ToString()) |> Real

        opt.Add(t >=. RealVal(0))
        opt.Add(x + t*dx =. h_x + t*h_dx)
        opt.Add(y + t*dy =. h_y + t*h_dy)
        opt.Add(z + t*dz =. h_z + t*h_dz)
        1 |> ignore

    opt.CheckOrFail()
    let resX = (opt.Eval x).ToString() |> int64 |> bigint
    let resY = (opt.Eval y).ToString() |> int64 |> bigint
    let resZ = (opt.Eval z).ToString() |> int64 |> bigint
    
    let resDX = (opt.Eval dx).ToString() |> int64 |> bigint
    let resDY = (opt.Eval dy).ToString() |> int64 |> bigint
    let resDZ = (opt.Eval dz).ToString() |> int64 |> bigint

    let res = [resX;resY;resZ] |> List.reduce (fun a b -> bigint.Add(a,b))
    res
    