module Day19

open System.Collections.Generic

type Dir = A | B

type Part = { x: int; m: int; a: int; s: int; }
type PartRange = { minX:int64; maxX:int64; minM:int64; maxM:int64; minA:int64; maxA:int64; minS:int64; maxS:int64; }
type Term = { prop: char Option; comparison: char option; num: int option; target: string; }
type Flow = { name: string; terms: Term list }

let processPart (flowMap: Map<string, Flow>) (part:Part) =
    "in"
    |> Seq.unfold(fun (state) ->
        if state="A" || state="R" then None else
        let flow = flowMap[state]
        let finalState = 
            flow.terms
            |> List.fold (fun (statei:string) term -> 
                if statei<>state then statei else
                if term.prop.IsNone then term.target else
                let partVal = match term.prop.Value with | 'x' -> part.x | 'm' -> part.m | 'a' -> part.a | 's' -> part.s | _ -> failwith "huh"
                let isConditionMatched = 
                    match term.comparison.Value with
                    | '>' -> partVal > term.num.Value
                    | '<' -> partVal < term.num.Value
                    | _ -> failwith "huh"

                if isConditionMatched=true then term.target else statei
            ) state
        Some(finalState,finalState)
    )
    |> Seq.last

let parseFlowMap (input:string) =
    let grps = input |> fun txt -> txt.Split(System.Environment.NewLine + System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
    let lns1 = grps[0].Split(System.Environment.NewLine) |> Seq.filter((<>)"") |> Seq.toList;
    
    let splitters = ['{';',';'}';] |> Array.ofList
    let flowMap = 
        lns1 
        |> Seq.map(fun ln1 -> 
            let name :: terms = ln1.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
            
            let flow0 = { name=name; terms=[]; }
            
            let finalFlow =
                terms
                |> List.fold (fun flow str -> 
                    let termSplitters = [':'] |> Array.ofList
                    let parts = str.Split(termSplitters) |> List.ofArray
                    match parts with
                    | condition :: target :: [] -> 
                        let prop = condition[0]
                        let comparison = condition[1]
                        let num = int(condition.Substring(2))
                        { flow with terms = {target=target; prop= Some prop; comparison =Some comparison; num=Some num;} :: flow.terms; }
                    | target :: [] -> { flow with terms={target=target; prop = None; comparison=None; num=None;} :: flow.terms }
                    | _ -> failwith "huh"
                ) flow0
            { finalFlow with terms = (finalFlow.terms |> List.rev) }
        )
        |> Seq.map(fun flow -> (flow.name, flow))
        |> Map
    flowMap

let parseParts (input:string) =
    let grps = input |> fun txt -> txt.Split(System.Environment.NewLine + System.Environment.NewLine, System.StringSplitOptions.RemoveEmptyEntries)
    let lns2 = grps[1].Split(System.Environment.NewLine) |> Seq.filter((<>)"") |> Seq.toList;

    let parts =
        lns2
        |> Seq.map(fun ln ->
            let partSplitters = ['{';'x';'=';',';'m';'a';'s';'}'] |> Array.ofList
            let partsStr = ln.Split(partSplitters, System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
            match partsStr with
            | [x;m;a;s] -> { x=int x;m=int m;a=int a;s= int s; }
            | _ -> failwith "huh"
        )
    parts
    

let solve1 inputPath = 
    let input = System.IO.File.ReadAllText(inputPath)
    let flowMap = parseFlowMap input
    let parts = parseParts input

    let acceptedParts = parts |> Seq.filter (processPart flowMap >> (=)"A")

    let res = acceptedParts |> Seq.map(fun part -> part.x + part.m + part.a + part.s) |> Seq.sum
    res

let isInvalidRange (pr:PartRange) =
    pr.minX > pr.maxX 
    || pr.minM > pr.maxM 
    || pr.minA > pr.maxA 
    || pr.minS > pr.maxS

let rec processPartRange (flowMap: Map<string, Flow>) (state:string) (pr0:PartRange) : int64 =
    if isInvalidRange pr0 then 0 else
    if state="R" then 0 else
    if state="A" then 
        let dx = pr0.maxX - pr0.minX + 1L
        let dm = pr0.maxM - pr0.minM + 1L
        let da = pr0.maxA - pr0.minA + 1L
        let ds = pr0.maxS - pr0.minS + 1L
        dx*dm*da*ds
    else
    
    
    let flow = flowMap[state]
    
    let (_,r) = 
        flow.terms
        |> List.fold (fun ((pr:PartRange),(acc:int64)) (t:Term)  ->
    
            if t.prop.IsNone then (pr, acc + (processPartRange flowMap t.target pr)) else

            let num = int64 t.num.Value
            let propStr = "" + (string t.prop.Value) + (string t.comparison.Value)

            let prContinue = 
                match propStr with 
                | "x>" -> { pr with maxX=num }
                | "x<" -> { pr with minX=num }

                | "m>" -> { pr with maxM=num }
                | "m<" -> { pr with minM=num }

                | "a>" -> { pr with maxA=num }
                | "a<" -> { pr with minA=num }

                | "s>" -> { pr with maxS=num }
                | "s<" -> { pr with minS=num }

                | _ -> failwith "huh"

            let prSplit = 
                match propStr with 
                | "x>" -> { pr with minX=num+1L }
                | "x<" -> { pr with maxX=num-1L }

                | "m>" -> { pr with minM=num+1L }
                | "m<" -> { pr with maxM=num-1L }

                | "a>" -> { pr with minA=num+1L }
                | "a<" -> { pr with maxA=num-1L }

                | "s>" -> { pr with minS=num+1L }
                | "s<" -> { pr with maxS=num-1L }

                | _ -> failwith "huh"

            let appendSplit = processPartRange flowMap t.target prSplit

            (prContinue, appendSplit+acc)
        ) (pr0,0L)

    r

let solve2 inputPath =
    let input = System.IO.File.ReadAllText(inputPath)
    let flowMap = parseFlowMap input

    let pr0 = { minX=1;maxX=4000; minM=1;maxM=4000; minA=1;maxA=4000; minS=1;maxS=4000; }
    let res = processPartRange flowMap "in" pr0

    res