module Day25

open System.Collections.Generic
open Microsoft.FSharp.Core.Operators.Checked
open AdventOfCode2023.CSharpHelpers

let (.+.) (x,y) (dx,dy) = (x+dx,y+dy)

let addToMapSet (k: string) (v: string) (mp:Map<string, string Set>) : Map<string, string Set> =
    let be4 = if mp.ContainsKey(k) then mp[k] else Set.empty
    mp.Add(k, (be4.Add(v)))

let removeFromMapSet (k: string) (v: string) (mp:Map<string, string Set>) : Map<string, string Set> =
    let be4 = if mp.ContainsKey(k) then mp[k] else Set.empty
    mp.Add(k, (be4.Remove(v)))

let findConnectedSet (mp:Map<string, string Set>) (key: string) =
    let s0 = [key] |> Set
    let (set1,_) = 
        (s0, [key])
        |> Seq.unfold (fun ((s:Set<string>), (edges:string list)) -> 
            if edges.IsEmpty then None else

            let e::tailE = edges
            let nxt = mp[e] |> Seq.filter(fun ne -> not(s.Contains(ne))) |> List.ofSeq
            let nextS = nxt |> List.fold (fun (acc: string Set) n -> acc.Add(n)) s

            let r = (nextS, nxt@tailE)
            Some(r,r)
        )
        |> Seq.last
    set1
    
let getGroups (mp:Map<string, string Set>) : (int*int) option =
    let keys = mp.Keys |> List.ofSeq


    let set1 = findConnectedSet mp (keys[0])
    if Set.count(set1) = keys.Length then None else
    let set2 = findConnectedSet mp (keys |> Seq.except set1 |> Seq.head)
    if Set.count(set1) + Set.count(set2) < keys.Length 
    then None 
    else Some(Set.count(set1), Set.count(set2))

let fordFulkerson (mp:Map<string, string Set>) (a:string) (b:string) : int =
    let keys = mp.Keys |> List.ofSeq
    let idxByKey = keys |> List.indexed |> List.map(fun (a,b) -> (b,a)) |> Map
    let a2d = Array2D.zeroCreate (keys.Length) (keys.Length)

    let inta = idxByKey[a]
    let intb = idxByKey[b]

    for (k,vs) in (Map.toSeq mp) do
        for v in vs do
            let idx1 = idxByKey[k]
            let idx2 = idxByKey[v]
            a2d[idx1,idx2] <- 1
            a2d[idx2,idx1] <- 1

    MaxFlow.FordFulkerson(a2d, inta, intb)

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let splitters = " :".ToCharArray()
    let comps = lns |> List.map(fun ln -> ln.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray |> fun (head::tkns) -> head,tkns)
    let connMap = comps |> List.fold(fun (mp) (head,tkns) -> tkns |> List.fold(fun mpi tkn -> mpi |> addToMapSet tkn head |> addToMapSet head tkn) mp) (Map.empty)

    let wires = connMap |> Map.toSeq |> Seq.map(fun (k,vs) -> vs |> Seq.map(fun v -> (k,v))) |> Seq.concat |> List.ofSeq |> List.distinctBy(fun (a,b) -> [a;b] |> Set)
    let len = wires.Length

    let res = 

        seq { 0 .. (len-3) }
        |> Seq.map(fun i1 -> 
            let (a1,b1) = wires[i1]
            let map1 = connMap|> removeFromMapSet a1 b1 |> removeFromMapSet b1 a1
            let ff1 = (fordFulkerson map1 a1 b1)
            let r1 = 
                if ff1>2 then Seq.empty else

                seq { (i1+1) .. (len-2) }
                |> Seq.map(fun i2 -> 
                    
                    let (a2,b2) = wires[i2]
                    let map2 = map1 |> removeFromMapSet a2 b2 |> removeFromMapSet b2 a2
                    let ff2 = (fordFulkerson map2 a2 b2)
                    if ff2>1 then Seq.empty else

                    seq { (i2+1) .. (len-1) }
                    |> Seq.map(fun i3 -> 
                        let (a3,b3) = wires[i3]
                        let map3 = map2 |> removeFromMapSet a3 b3 |> removeFromMapSet b3 a3

                        let r = 
                            match getGroups map3 with
                            | None -> None
                            | Some((a,b)) -> Some(a*b)
                        r
                    )
                    |> Seq.choose id
                ) |> Seq.concat
            r1
        ) 
        |> Seq.concat
        |> Seq.head

    res