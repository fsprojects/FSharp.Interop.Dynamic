namespace Tests

/// Worked examples from docfx/docs/why.md. Keep them in lockstep with that page.
open FSharp.Interop.Dynamic
open Xunit
open System.Dynamic

module WhyExamples =

    [<Fact>]
    let ``untyped payload: read a field whose name is not in the FSharp type`` () =
        let payload = ExpandoObject()
        payload?userId <- "u-17"
        payload?role <- "admin"
        let userId: string = payload?userId
        Assert.Equal("u-17", userId)

    [<Fact>]
    let ``optional field on an untyped payload`` () =
        let payload = ExpandoObject()
        payload?userId <- "u-17"
        let role: string option = payload |> Dyn.tryGet "role"
        let userId: string option = payload |> Dyn.tryGet "userId"
        Assert.Equal(None, role)
        Assert.Equal(Some "u-17", userId)

    [<Fact>]
    let ``method name arrives as data`` () =
        let name = "ToUpper"
        let result: string = "ada" |> Dyn.invokeMember name ()
        Assert.Equal("ADA", result)

    type Client() =
        inherit DynamicObject()
        member val Last = "" with get, set
        override this.TryInvokeMember(binder, args, result) =
            this.Last <- sprintf "%s(%O)" binder.Name args.[0]
            result <- null
            true

    [<Fact>]
    let ``CSharp API handed back a DynamicObject`` () =
        let client = Client()
        let _: unit = client?addMessage("hi")
        Assert.Equal("addMessage(hi)", client.Last)
