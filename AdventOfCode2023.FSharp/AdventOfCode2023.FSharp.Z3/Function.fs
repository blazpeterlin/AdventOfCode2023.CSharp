﻿module Microsoft.Z3.Function

open System
open Microsoft.Z3
open Microsoft.Z3.Bool

type Microsoft.Z3.Bool.Z3 with
  static member inline CreateFunction< ^T when ^T: (static member FromExpr : Expr -> ^T)>
                          (name: string, range: Sort, [<ParamArray>] args: Theory []) =
    let exprs = args |> Array.map (fun arg -> arg.Expr)
    let sorts = exprs |> Array.map (fun expr -> expr.Sort)
    let expr = Gs.context().MkFuncDecl(name, sorts, range).Apply(exprs)
    (^T : (static member FromExpr : Expr -> ^T) (expr))


