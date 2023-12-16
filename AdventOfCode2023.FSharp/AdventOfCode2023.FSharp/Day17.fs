module Day17

open System.Collections.Generic

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let res = 1+1
    res

let solve2 inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let res = 1+1
    res