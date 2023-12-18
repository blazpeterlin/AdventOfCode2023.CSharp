module Day18

open System.Collections.Generic

type Ins = { dir: string; num: int64; clr: string; }

let (+..) (x,y) (dx,dy) = (x+dx,y+dy)
let (.*.) (x,y) mp =(x*mp,y*mp)

let paintMoveDir circularDir dirTpl =
    let (cwDirX, cwDirY) = 
        match dirTpl with
        | (1L,0L) -> (0L, 1L)
        | (0L,1L) -> (-1L, 0L)
        | (-1L,0L) -> (0L, -1L)
        | (0L, -1L) -> (1L,0L)
        | _ ->failwith "huh"
    let actualDir =
        match circularDir with
        | "CW" ->  (cwDirX, cwDirY)
        | "CCW" ->  (-cwDirX, -cwDirY)
        | _ ->failwith "huh"
    actualDir

let moveDir dir =
    match dir with
    | "L" -> (-1L,0L)
    | "R" -> (1L,0L)
    | "U" -> (0L, -1L)
    | "D" -> (0L, 1L)
    | _ ->failwith "huh"

let toPolygonPoints (listIns: Ins list) =
    listIns
    |> List.scan (fun pos ins ->
        let md = moveDir ins.dir
        (md .*. ins.num) +.. pos
    ) (0L,0L)
    |> List.ofSeq

let solve1 inputPath = 
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let remEmpty = System.StringSplitOptions.RemoveEmptyEntries
    let split = [' ';'(';')'] |> Array.ofList
    let listIns = 
        lns 
        |> List.map(fun ln -> 
            let tknList = ln.Split(split, remEmpty) |> List.ofArray 
            match tknList with 
            | [dir;numStr;clr] -> { dir=dir; num=int64(numStr); clr=clr; } 
            | _ -> failwith "huh"
        )

    let pp = toPolygonPoints listIns
    let ppAreaDouble = 
        pp 
        |> List.pairwise 
        |> List.map (fun ((x1,y1),(x2,y2)) -> 
            decimal(x1*y2-y1*x2)
        )
        |> List.sum
    let wallSize = listIns |> List.map(_.num) |> List.sum
    let resAttempt2 = wallSize/2L+ int64(ppAreaDouble/2M) + 1L
    
    resAttempt2

let solve2 inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;

    let remEmpty = System.StringSplitOptions.RemoveEmptyEntries
    let split = [' ';'(';')';'#'] |> Array.ofList
    let listIns = 
        lns 
        |> List.map(fun ln -> 
            let tknList = ln.Split(split, remEmpty) |> List.ofArray 
            match tknList with 
            | [_;_;realStr] -> 
                let dir = match Array.last(realStr.ToCharArray()) with | '0' -> "R" | '1' -> "D" | '2' -> "L" | '3' -> "U" | _ -> failwith "huh"
                let numStr = "0x" + realStr.Substring(0, realStr.Length-1)
                let num = System.Convert.ToInt64(numStr, 16)
                let clr = ""
                { dir=dir; num=num; clr=clr; } 
            | _ -> failwith "huh"
        )

    
    let pp = toPolygonPoints listIns
    let ppAreaDouble = 
        pp 
        |> List.pairwise 
        |> List.map (fun ((x1,y1),(x2,y2)) -> 
            decimal(x1*y2-y1*x2)
        )
        |> List.sum
    let wallSize = listIns |> List.map(_.num) |> List.sum
    let res = wallSize/2L+ int64(ppAreaDouble/2M) + 1L
    res