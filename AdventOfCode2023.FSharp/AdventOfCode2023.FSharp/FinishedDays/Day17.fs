module Day17

open System.Collections.Generic

let dirs = [(-1,0);(1,0);(0,-1);(0,1)]

let (+..) (x,y) (dx,dy) =
    (x+dx,y+dy)


type Edge = { pos: int*int; costSoFar: int; lastDir: int*int; lastDirAmount: int; }
type State = { edges: Edge list; visited: Map<Edge,int>; }

let edge2key edge = { edge with costSoFar=0; }
let edge2val edge = edge.costSoFar

let nextState acceptableDirFilter (coordMap:Map<int*int,int>) (st: State) _iter =
    let nextEdges = 
        st.edges
        |> Seq.map(fun (edge:Edge) ->
            let pos = edge.pos
            if not(Map.containsKey pos coordMap) then [] else

            let acceptableDirs = 
                dirs 
                |> Seq.filter(acceptableDirFilter edge)
            let acceptableEdges = 
                acceptableDirs
                |> Seq.map(fun dir -> { edge with pos = pos +.. dir; lastDir = dir; lastDirAmount = if edge.lastDir=dir then edge.lastDirAmount+1 else 1;  })
                |> Seq.filter(fun edge -> Map.containsKey edge.pos coordMap)
                |> Seq.map(fun edge -> { edge with costSoFar = edge.costSoFar + coordMap[edge.pos]})
                
                |> Seq.filter(fun edge -> if Map.containsKey (edge2key(edge)) st.visited then st.visited[edge2key(edge)] > (edge2val(edge)) else true)
            acceptableEdges |> Seq.toList
        )
        |> List.concat

    let nextEdgesPruned = 
        nextEdges 
        |> Seq.groupBy (fun edg -> edge2key(edg))
        |> Seq.map(fun (key,grpList) -> grpList |> Seq.sortBy (fun edg -> edg.costSoFar) |> Seq.head)
        |> Seq.sortBy(fun edg -> (edg.pos,edg.lastDir))
        |> Seq.toList

    let nextVisited =
        nextEdgesPruned
        |> Seq.fold (fun map edg -> Map.add (edge2key(edg)) (edge2val(edg)) map) st.visited

    { edges = nextEdgesPruned; visited = nextVisited; }

let parseMap inputPath = 

    let lns = System.IO.File.ReadAllLines(inputPath) |> Seq.filter((<>)"") |> Seq.toList;
   
    let coordMap = 
        lns 
        |> Seq.indexed 
        |> Seq.map(fun (y,ln) -> 
            ln.ToCharArray() 
            |> Seq.indexed 
            |> Seq.map(fun (x,ch) -> ((x,y), int(new string([ch] |> Array.ofList))) )
        )
        |> Seq.concat
        |> Map.ofSeq

    coordMap

let acceptableDirFilter_NormalCrucible edge (dirX,dirY) =
    (edge.lastDirAmount < 3 || (dirX,dirY) <> edge.lastDir)
    && (-dirX,-dirY) <> edge.lastDir // don't turn 180 degrees

let acceptableDirFilter_UltraCrucible edge (dirX,dirY) =
    (edge.lastDirAmount < 10 || (dirX,dirY) <> edge.lastDir)
    && (-dirX,-dirY) <> edge.lastDir // don't turn 180 degrees
    && (edge.lastDirAmount >= 4 || edge.lastDir = (0,0) || edge.lastDir = (dirX,dirY)) // ultra speed

let solve (coordMap:Map<int*int,int>) acceptableDirFilter =
    let initialEdge = { pos=(0,0); costSoFar=0; lastDir=(0,0); lastDirAmount=0 }
    let st0 = { edges= [initialEdge]; visited= [initialEdge,0] |> Map.ofList }
    let maxX = coordMap.Keys |> Seq.map(fun (x,_) -> x) |> Seq.max
    let maxY = coordMap.Keys |> Seq.map(fun (_,y) -> y) |> Seq.max
    let maxSteps = coordMap.Keys.Count

    let r = 
        seq { 0 .. maxSteps }
        |> Seq.fold (nextState acceptableDirFilter coordMap) st0
    let resKvps = r.visited.Keys |> Seq.filter (fun edge -> edge.pos = (maxX,maxY)) |> List.ofSeq

    (r, resKvps)

let solve1 inputPath = 
    let coordMap = parseMap inputPath

    let (r, resKvps) = solve coordMap acceptableDirFilter_NormalCrucible
    
    let resKvp = resKvps |> Seq.minBy (fun edge -> r.visited[edge])
    let res = r.visited[resKvp]
    res

let solve2 inputPath =
    let coordMap = parseMap inputPath

    let (r, resKvps) = solve coordMap acceptableDirFilter_UltraCrucible

    let resKvp = resKvps |> Seq.filter(fun edge -> edge.lastDirAmount >= 4) |> Seq.minBy (fun edge -> r.visited[edge])
    let res = r.visited[resKvp]
    res