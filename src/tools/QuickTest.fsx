module QuickTest

// Use this template to make quick tests when adding new features to Fable.
// You must run a full build at least once (from repo root directory,
// type `sh build.sh` on OSX/Linux or just `build` on Windows). Then:
// - When making changes to Fable.Compiler run `build QuickFableCompilerTest`
// - When making changes to fable-core run `build QuickFableCoreTest`

// Please don't add this file to your commits

#r "../../build/fable/Fable.Core.dll"
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Core.Testing

let equal expected actual =
    let areEqual = expected = actual
    printfn "%A = %A > %b" expected actual areEqual
    if not areEqual then
        failwithf "Expected %A but got %A" expected actual

// Write here your unit test, you can later move it
// to Fable.Tests project. For example:
// [<Test>]
// let ``My Test``() =
//     Seq.except [2] [1; 3; 2] |> Seq.last |> equal 3
//     Seq.except [2] [2; 4; 6] |> Seq.head |> equal 4

// You'll have to run your test manually, sorry!
// ``My Test``()

let f (x:obj) (y:obj) (z:obj) = (string x) + (string y) + (string z)

[<Test>]
let ``Mapping from values to functions works``() =
    let a = [| "a"; "b"; "c" |]
    let b = [| 1; 2; 3 |]
    let concaters1 = a |> Array.map (fun x y -> y + x)
    let concaters2 = a |> Array.map (fun x -> (fun y -> y + x))
    let concaters3 = a |> Array.map (fun x -> let f = (fun y -> y + x) in f)
    let concaters4 = a |> Array.map f
    let concaters5 = b |> Array.mapi f
    concaters1.[0] "x" |> equal "xa"
    concaters2.[1] "x" |> equal "xb"
    concaters3.[2] "x" |> equal "xc"
    concaters4.[0] "x" "y" |> equal "axy"
    concaters5.[1] "x" |> equal "12x"
    let f2 = f
    a |> Array.mapi f2 |> Array.item 2 <| "x" |> equal "2cx"

``Mapping from values to functions works``()
