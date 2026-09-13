namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
open Xunit
open FsUnit.Xunit
open System
open System.Collections.Generic
open System.Dynamic
open Microsoft.CSharp.RuntimeBinder

#nowarn "44"

type Echo() =
    member _.Of<'T>(x: 'T) = x

type Touch() =
    member val Called = false with get, set
    member this.Go() = this.Called <- true

type Holder() =
    member _.Inc = fun (x: int) -> x + 1

type Boom() =
    member _.Go() : unit = invalidOp "nope"

type PropThrows() =
    member _.Bad with get() = invalidOp "bad getter"

module Invocation =

    [<Fact>]
    let ``invocation Direct of a non-function yields the target, converted`` () =
        let n: int = Dyn.invocation (box 42) Direct
        n |> should equal 42

    [<Fact>]
    let ``invokeDirect applies an FSharp function`` () =
        let add = (+)
        let add3: int -> int = add |> Dyn.invokeDirect 3
        add3 4 |> should equal 7

    [<Fact>]
    let ``invokeDirect of a unit FSharp function runs it`` () =
        let n = ref 0
        let bump () = n := !n + 1
        let _: unit = bump |> Dyn.invokeDirect ()
        !n |> should equal 1

    [<Fact>]
    let ``invokeDirect of a non-callable wraps both binder failures`` () =
        (fun () -> "hello" |> Dyn.invokeDirect 1 |> ignore)
        |> should throw typeof<AggregateException>

    [<Fact>]
    let ``invoke with None is direct invocation`` () =
        let add = (+)
        let add3: int -> int = Dyn.invoke add None 3
        add3 4 |> should equal 7

    [<Fact>]
    let ``invokeGeneric calls an instance generic method`` () =
        let actual: string = Echo() |> Dyn.invokeGeneric "Of" [typeof<string>] "xyz"
        actual |> should equal "xyz"

    [<Fact>]
    let ``invokeMember with unit invokes a parameterless method`` () =
        let t = Touch()
        t |> Dyn.invokeMember "Go" ()
        t.Called |> should equal true

    [<Fact>]
    let ``get as a function value reads a property`` () =
        let target = ExpandoObject()
        target |> Dyn.set "Name" "Ada"
        let readName = Dyn.get "Name"
        (target |> readName) |> should equal "Ada"

    [<Fact>]
    let ``invokeMember as a function value calls the method`` () =
        let substring = Dyn.invokeMember "Substring"
        "HelloWorld" |> substring (0, 5) |> should equal "Hello"

    [<Fact>]
    let ``missing member wraps both binder failures`` () =
        let ex =
            Assert.Throws<AggregateException>(fun () ->
                (box 1) |> Dyn.invokeMember "Nope" () |> ignore)
        ex.InnerExceptions.Count |> should equal 2
        ex.InnerExceptions
        |> Seq.iter (fun inner -> inner |> should be instanceOfType<RuntimeBinderException>)

    [<Fact>]
    let ``staticTarget and staticContext are interchangeable for a static call`` () =
        let viaGeneric: int seq = Dyn.staticTarget<Linq.Enumerable> |> Dyn.invokeGeneric "Empty" [typeof<int>] ()
        let viaType: int seq = typeof<Linq.Enumerable> |> Dyn.staticContext |> Dyn.invokeGeneric "Empty" [typeof<int>] ()
        viaGeneric |> should equal Seq.empty<int>
        viaType |> should equal Seq.empty<int>

    [<Fact>]
    let ``namedArg marks an argument for DLR named invocation`` () =
        let arg = Dyn.namedArg "Two" 2
        arg.Name |> should equal "Two"
        arg.Value |> should equal 2

    [<Fact>]
    let ``explicitConvert as a function value uses the inferred target type`` () =
        let toDecimal: int -> decimal = Dyn.explicitConvert
        toDecimal 50 |> should equal 50M

    [<Fact>]
    let ``implicitConvert as a function value uses the inferred target type`` () =
        let toDecimal: int -> decimal = Dyn.implicitConvert
        toDecimal 50 |> should equal 50M

    [<Fact>]
    let ``getChain on a missing link raises a binder exception`` () =
        let target = ExpandoObject()
        (fun () -> target |> Dyn.getChain ["NoSuch"] |> ignore)
        |> should throw typeof<RuntimeBinderException>

    [<Fact>]
    let ``invocation GenericMember of a non-function gets the member by name`` () =
        let length: int = Dyn.invocation "Hello" (GenericMember("Length", Array.empty))
        length |> should equal 5

    [<Fact>]
    let ``invokeGeneric falls back to an FSharp function member`` () =
        let actual: int = Holder() |> Dyn.invokeGeneric "Inc" [typeof<int>] 5
        actual |> should equal 6

    [<Fact>]
    let ``a member that throws is not wrapped as a binder failure`` () =
        (fun () ->
            let _: unit = Boom() |> Dyn.invokeMember "Go" ()
            ())
        |> should throw typeof<InvalidOperationException>

    [<Fact>]
    let ``a throwing getter is not wrapped after a binder failure`` () =
        (fun () -> PropThrows() |> Dyn.invokeMember "Bad" () |> ignore)
        |> should throw typeof<InvalidOperationException>
