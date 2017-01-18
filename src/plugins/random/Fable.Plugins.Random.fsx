namespace Fable.Plugins

#r "../../../build/fable-core/Fable.Core.dll"

open Fable
open Fable.AST
open Fable.AST.Fable.Util

type RandomPlugin() =
    interface IReplacePlugin with
        member x.TryReplace _com (info: Fable.ApplyInfo) =
            match info.ownerFullName with
            | "System.Random" ->
                match info.methodName with
                | ".ctor" ->
                    // makeJsObject is one of the helpers to emit Fable AST
                    // in the Fable.AST.Fable.Util module
                    makeJsObject info.range [] |> Some
                | "Next" ->
                    let min, max =
                        match info.args with
                        | [] -> makeConst 0, makeConst System.Int32.MaxValue
                        | [max] -> makeConst 0, max
                        | [min; max] -> min, max
                        | _ -> failwith "Unexpected arg count for Random.Next"
                    "Math.floor(Math.random() * ($1 - $0)) + $0"
                    |> makeEmit info.range info.returnType [min; max]
                    |> Some
                | _ -> None
            | _ -> None
