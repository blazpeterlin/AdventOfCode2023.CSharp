module Day22

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked

let (.+.) (x,y) (dx,dy) = (x+dx,y+dy)
type Dir = A | B

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let res = 1+1
    res

let solve2 inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let res = 1+1
    res