namespace Tests

open FSharp.Interop.Dynamic
open Xunit
open FsUnit.Xunit
open System
open System.Collections.Generic
open System.Dynamic
open Microsoft.CSharp.RuntimeBinder

type Wide() =
    member _.Sum15(a1:int, a2:int, a3:int, a4:int, a5:int, a6:int, a7:int, a8:int,
                   a9:int, a10:int, a11:int, a12:int, a13:int, a14:int, a15:int) =
        a1+a2+a3+a4+a5+a6+a7+a8+a9+a10+a11+a12+a13+a14+a15
    member _.Sum17(a1:int, a2:int, a3:int, a4:int, a5:int, a6:int, a7:int, a8:int, a9:int,
                   a10:int, a11:int, a12:int, a13:int, a14:int, a15:int, a16:int, a17:int) =
        a1+a2+a3+a4+a5+a6+a7+a8+a9+a10+a11+a12+a13+a14+a15+a16+a17
    member val Seen = 0 with get, set
    member this.Take15(a1:int, a2:int, a3:int, a4:int, a5:int, a6:int, a7:int, a8:int,
                       a9:int, a10:int, a11:int, a12:int, a13:int, a14:int, a15:int) : unit =
        this.Seen <- a1+a2+a3+a4+a5+a6+a7+a8+a9+a10+a11+a12+a13+a14+a15

type Nested() =
    member val Items = ResizeArray<string>(["zero"; "one"]) with get
    member val Dict = Dictionary<string, int>(dict ["k", 7]) with get
    member val Name = "nested" with get, set

type Outer() =
    member val Inner = Nested() with get

/// Paths that used to be covered only by Dynamitey's own tests: the chain
/// parser, the more-than-14-argument fallback, and the types Dyn hands back.
module BridgeSupport =

    [<Fact>]
    let ``getChain walks getters, int indexers and string indexers`` () =
        let outer = Outer()
        (outer |> Dyn.getChain ["Inner"; "Name"] : string) |> should equal "nested"
        (outer |> Dyn.getChain ["Inner"; "Items[1]"] : string) |> should equal "one"
        (outer |> Dyn.getChain ["Inner"; "Dict['k']"] : int) |> should equal 7

    [<Fact>]
    let ``setChain sets through getters and indexers`` () =
        let outer = Outer()
        outer |> Dyn.setChain ["Inner"; "Name"] "changed"
        outer.Inner.Name |> should equal "changed"
        outer |> Dyn.setChain ["Inner"; "Items[0]"] "nil"
        outer.Inner.Items.[0] |> should equal "nil"
        outer |> Dyn.setChain ["Inner"; "Dict['k']"] 8
        outer.Inner.Dict.["k"] |> should equal 8

    [<Fact>]
    let ``setChain on an unparsable chain raises FormatException`` () =
        Assert.Throws<FormatException>(fun () ->
            Outer() |> Dyn.setChain ["!!"] 1) |> ignore

    [<Fact>]
    let ``invokeMember with 15 arguments returns a value`` () =
        let w = Wide()
        let total: int = w |> Dyn.invokeMember "Sum15" (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15)
        total |> should equal 120

    [<Fact>]
    let ``invokeMember with 17 arguments returns a value`` () =
        let w = Wide()
        let total: int = w |> Dyn.invokeMember "Sum17" (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17)
        total |> should equal 153

    [<Fact>]
    let ``invokeMember with 15 arguments can return unit`` () =
        let w = Wide()
        w |> Dyn.invokeMember "Take15" (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15)
        w.Seen |> should equal 120

    [<Fact>]
    let ``a missing member with 15 arguments still surfaces RuntimeBinderException`` () =
        let ex =
            Assert.Throws<AggregateException>(fun () ->
                (Wide() |> Dyn.invokeMember "Nope" (1,2,3,4,5,6,7,8,9,10,11,12,13,14,15) : int) |> ignore)
        ex.InnerExceptions |> Seq.iter (fun inner -> inner |> should be instanceOfType<RuntimeBinderException>)

    [<Fact>]
    let ``a direct dynamic invoke with 15 arguments works`` () =
        let ex = ExpandoObject()
        let f = Func<int,int,int,int,int,int,int,int,int,int,int,int,int,int,int,int>(
                    fun a b c d e f g h i j k l m n o -> a+b+c+d+e+f+g+h+i+j+k+l+m+n+o)
        ex |> Dyn.set "Sum" f
        let total: int = ex?Sum(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15)
        total |> should equal 120

    [<Fact>]
    let ``namedArg and staticTarget return BridgeSupport types`` () =
        (Dyn.namedArg "x" 1).GetType()
        |> should equal typeof<FSharp.Interop.Dynamic.BridgeSupport.InvokeArg>
        Dyn.staticTarget<string>.GetType()
        |> should equal typeof<FSharp.Interop.Dynamic.BridgeSupport.InvokeContext>
