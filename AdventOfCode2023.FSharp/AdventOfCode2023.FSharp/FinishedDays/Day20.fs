module Day20

open Microsoft.FSharp.Core.Operators.Checked
open System.Collections.Generic

type Broadcaster = B
and FlipFlop = { state: bool  }
and Conjunction = { pulsesReceived: Map<string, bool> }
and ModuleType = | BR of Broadcaster | FF of FlipFlop | CJ of Conjunction

type Module = { id: string; targets: string list; moduleType: ModuleType; }

type Signal = { from: string; target: string; v: bool; }
type State = { ms: Map<string, Module>; sigs: Signal list; sighistory: Signal list; keepHistory: bool; }

let advance (argSt: State) : State =
    let ((nextMs: Map<string, Module>), (nextSigsRev: Signal list)) =
        argSt.sigs
        |> List.fold (fun (ms,nextSigsAcc) (signal:Signal) -> 
            if not(ms.ContainsKey signal.target) then (ms,nextSigsAcc) else
            let m = ms[signal.target]
            let nextM = 
                match m.moduleType with
                | FF(f) -> 
                    if signal.v then m else
                    { m with moduleType = FF({ state=not f.state; }) }
                | BR(_) -> m
                | CJ(c) ->
                    { m with moduleType = CJ({ pulsesReceived = (Map.add signal.from signal.v c.pulsesReceived) }) }
            let nextMs = Map.add signal.target nextM ms
            let nextSigsInc = 
                if not(argSt.keepHistory) then [] else
                m.targets
                |> List.choose(fun t -> 
                    let nextV = 
                        match nextM.moduleType with
                        | BR(_) -> Some(signal.v)
                        | FF(f) -> if signal.v then None else Some(f.state)
                        | CJ(c) -> if Seq.forall ((=)true) c.pulsesReceived.Values then Some(false) else Some(true)
                    if nextV.IsNone then None else
                    Some({ from= m.id; target=t; v=nextV.Value; })
                )
            (nextMs, nextSigsInc@nextSigsAcc)
        ) (argSt.ms,[])

    { ms=nextMs; sigs=(List.rev nextSigsRev); sighistory=argSt.sigs@argSt.sighistory; keepHistory=argSt.keepHistory; }


let pushBtn (st:State) =
    let stWithBtn = { st with sigs=[{ from= "button"; target="broadcaster"; v= false; }]; }
    stWithBtn
    |> Seq.unfold(fun st -> 
        let st2 = advance st
        if st2=st
        then None
        else Some((st2,st2))
    )
    |> Seq.last

let parseModules inputPath =
    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
    let splitters = [',';' ';'-';'>'] |> Array.ofList
    let cfgs = lns |> Seq.map(fun ln -> ln.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray) |> List.ofSeq
    let modulesInput = 
        cfgs
        |> Seq.map(
            fun cfg -> 
                match cfg with
                | name :: tgts -> 
                    let ch = name[0]
                    match ch with
                    | '%' -> { moduleType=FF({ state=false }); id=name.Substring(1); targets=tgts; }
                    | '&' -> { moduleType=CJ({ pulsesReceived=Map.empty; }); id=name.Substring(1); targets=tgts; }
                    | 'b' -> { moduleType=BR(B); id=name; targets=tgts; }
                    | _ -> failwith "huh"
                | _ -> failwith "huh"
        )
        |> List.ofSeq
    modulesInput

let processModuleMap modulesInput =
    let sourcesByTargets = modulesInput |> List.map(fun m -> m.targets |> List.map(fun t -> (t,m.id))) |> List.concat

    let moduleMap = 
        modulesInput
        |> List.map(fun m -> 
            match m.moduleType with
            | CJ(c) -> 
                let sourcesForMap = sourcesByTargets |> List.filter(fun (tgt,src) -> tgt = m.id) |> List.map(snd) |> List.map(fun src -> (src,false))
                { m with moduleType=CJ({ pulsesReceived = Map(sourcesForMap) }) }
            | _ -> m
        )
        |> Seq.map(fun m -> (m.id, m)) 
        |> Map

    moduleMap
    
let solve1 inputPath = 
    let modulesInput = parseModules inputPath
    let moduleMap = processModuleMap modulesInput
    let state0 = { ms=moduleMap; sighistory=[]; sigs=[]; keepHistory=true; }

    let stateN = 
        seq { 1 .. 1000 }
        |> Seq.fold (fun st _ -> pushBtn st) state0

    let highs = stateN.sighistory |> Seq.filter(fun signal -> signal.v = true) |> Seq.length
    let lows = stateN.sighistory |> Seq.filter(fun signal -> signal.v = false) |> Seq.length

    let res = highs * lows

    res

    
let rec gcd x y =
    if y = 0 then x
    else gcd y (x % y)
    
let lcm a b = a*b/(gcd a b)

let solve2 inputPath =
    let modulesInput = parseModules inputPath
    let moduleMap = processModuleMap modulesInput
    let state0 = { ms=moduleMap; sighistory=[]; sigs=[]; keepHistory=true; }
    

    // grabbed manually from input
    let keys = ["vr";"gt";"nl";"lr"]

    let firstFinds = 
        keys
        |> List.map(fun key -> 
            Seq.initInfinite (fun i -> i+1)
            |> Seq.scan(fun (_,st) i -> (i, pushBtn { st with sighistory=[] })) (0,state0)
            |> Seq.takeWhile(fun (i,st) -> i <= 1 || st.sighistory |> List.tryFind (fun elt -> elt.from=key && elt.v) |> (=)None)
            |> Seq.last
            |> fst
        )
        |> List.map int64

    // &vr -> jq  3906 7813 11720 15627     ... 3907
    // &gt -> jq  3910 7821 11732           ... 3911
    // &nl -> jq  4002 8005                 ... 4003
    // &lr -> jq  3888 7777                 ... 3889

    let res = (firstFinds |> List.map(fun ff -> ff+1L) |> List.reduce (*))
    // 237878264003759

    res