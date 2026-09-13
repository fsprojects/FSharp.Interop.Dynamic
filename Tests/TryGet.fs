namespace Tests

open FSharp.Interop.Dynamic
open Xunit
open FsUnit.Xunit
open System
open System.Dynamic

type Named() =
    member _.Name = "Ada"

module TryGet =

    [<Fact>]
    let ``tryGet returns Some for a present Expando property`` () =
        let o = ExpandoObject()
        o |> Dyn.set "myProp" "hi"
        Assert.Equal(Some "hi", o |> Dyn.tryGet "myProp")

    [<Fact>]
    let ``tryGet returns None for a missing Expando property`` () =
        let o = ExpandoObject()
        let missing: string option = o |> Dyn.tryGet "myProp"
        missing |> should equal None

    [<Fact>]
    let ``exists is true for a present Expando property`` () =
        let o = ExpandoObject()
        o |> Dyn.set "myProp" "hi"
        o |> Dyn.exists "myProp" |> should equal true

    [<Fact>]
    let ``exists is false for a missing Expando property`` () =
        let o = ExpandoObject()
        o |> Dyn.exists "myProp" |> should equal false

    [<Fact>]
    let ``tryGet and exists see a CLR property`` () =
        let o = Named()
        Assert.Equal(Some "Ada", o |> Dyn.tryGet "Name")
        o |> Dyn.exists "Name" |> should equal true

    [<Fact>]
    let ``tryGet and exists miss a CLR property that is not there`` () =
        let o = Named()
        let missing: string option = o |> Dyn.tryGet "NoSuch"
        missing |> should equal None
        o |> Dyn.exists "NoSuch" |> should equal false

    [<Fact>]
    let ``a present null value exists and tryGet returns Some null`` () =
        let o = ExpandoObject()
        o |> Dyn.set "myProp" null
        o |> Dyn.exists "myProp" |> should equal true
        let value: obj option = o |> Dyn.tryGet "myProp"
        value |> should equal (Some null)

    [<Fact>]
    let ``a throwing getter is not turned into None`` () =
        (fun () -> PropThrows() |> Dyn.tryGet "Bad" |> ignore)
        |> should throw typeof<InvalidOperationException>

    [<Fact>]
    let ``a throwing getter is not reported as missing by exists`` () =
        (fun () -> PropThrows() |> Dyn.exists "Bad" |> ignore)
        |> should throw typeof<InvalidOperationException>
