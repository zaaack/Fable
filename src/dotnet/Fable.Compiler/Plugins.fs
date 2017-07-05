namespace Fable
open Fable.AST

type IReplacePlugin =
    inherit IPlugin
    abstract TryReplace: com: ICompiler -> info: Fable.ApplyInfo -> Fable.Expr option

module Plugins =
    let tryPlugin<'T,'V when 'T:>IPlugin> (r: SourceLocation option) (f: 'T->'V option) =
        Seq.tryPick (fun (path: string, plugin: 'T) ->
            try f plugin
            with
            | ex when Option.isSome r -> System.Exception(sprintf "Error in plugin %s: %s %O" path ex.Message r.Value, ex) |> raise
            | ex -> System.Exception(sprintf "Error in plugin %s: %s" path ex.Message, ex) |> raise)
