namespace Tests

open FSharp.Interop.Dynamic.SymbolicString
open Xunit
open FsUnit.Xunit
open System

module Quotations =

    [<Fact>]
    let ``sym throws if evaluated instead of quoted`` () =
        (fun () -> sym<int> |> ignore)
        |> should throw typeof<Exception>

    [<Fact>]
    let ``nameOf rejects an integer literal, which has no name`` () =
        (fun () -> Symbol.nameOf(1) |> ignore)
        |> should throw typeof<ArgumentException>

    [<Fact>]
    let ``nameOf rejects a tuple expression`` () =
        (fun () -> Symbol.nameOf((1, 2)) |> ignore)
        |> should throw typeof<ArgumentException>

    [<Fact>]
    let ``nameOf rejects an if-then-else expression`` () =
        (fun () -> Symbol.nameOf(if true then 1 else 0) |> ignore)
        |> should throw typeof<ArgumentException>

    [<Fact>]
    let ``typeOf matches nameOf's rejection of an unnamed expression`` () =
        (fun () -> Symbol.typeOf(1) |> ignore)
        |> should throw typeof<ArgumentException>
