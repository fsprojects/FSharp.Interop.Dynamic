namespace Tests

/// Snippets from README.md. If you change a working example in the README,
/// change the matching fact here so CI still proves it.
open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
open Xunit
open System.Dynamic

module ReadmeExamples =

    [<Fact>]
    let ``quick start: Expando get/set and Substring`` () =
        let o = ExpandoObject()
        o?Name <- "Ada"
        let name: string = o?Name
        let hello: string = "HelloWorld"?Substring(0, 5)
        Assert.Equal("Ada", name)
        Assert.Equal("Hello", hello)

    [<Fact>]
    let ``void CLR methods need a unit annotation`` () =
        let items = ResizeArray<string>()
        let _: unit = items?Add("x")
        Assert.Equal("x", items.[0])

    [<Fact>]
    let ``tryGet and exists on the same Expando`` () =
        let o = ExpandoObject()
        o?Name <- "Ada"
        let present: string option = o |> Dyn.tryGet "Name"
        let missing: string option = o |> Dyn.tryGet "NoSuch"
        Assert.Equal(Some "Ada", present)
        Assert.Equal(None, missing)
        Assert.True(o |> Dyn.exists "Name")
        Assert.False(o |> Dyn.exists "NoSuch")

    [<Fact>]
    let ``pipe through Dyn`` () =
        let o = ExpandoObject()
        o |> Dyn.set "Name" "Ada"
        let name: string = o |> Dyn.get "Name"
        let hello: string = "HelloWorld" |> Dyn.invokeMember "Substring" (0, 5)
        Assert.Equal("Ada", name)
        Assert.Equal("Hello", hello)

    [<Fact>]
    let ``binary operators including mixed types and reduce`` () =
        let n: int = 5 ?+? 4
        let f: float = 5 ?+? 3.5
        let s: string = "Hello" ?+? " World"
        let folded = [1; 2; 3; 4] |> List.reduce (?+?)
        Assert.Equal(9, n)
        Assert.Equal(8.5, f)
        Assert.Equal("Hello World", s)
        Assert.Equal(10, folded)

    [<Fact>]
    let ``direct invoke of an FSharp function`` () =
        let add3: int -> int = !?(+) 3
        Assert.Equal(7, add3 4)
