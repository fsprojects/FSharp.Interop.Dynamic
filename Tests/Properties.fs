namespace Tests

open System.Dynamic
open FsCheck
open FsCheck.FSharp
open Xunit
open FSharp.Interop.Dynamic

/// Property-based tests: FsCheck feeds generated member names and values
/// through the dynamic binder instead of a handful of fixed examples.
/// Each property runs inside a Fact via Check.One, so it works with the
/// xUnit runner this project already uses.
module Properties =

    type Clr() =
        member val Name = "Ada" with get, set

    let private check (property: 'Property) =
        Check.One(Config.QuickThrowOnFailure.WithMaxTest 500, property)

    [<Fact>]
    let ``set then get returns the value on an Expando`` () =
        check (fun (NonNull (name: string)) (value: int) ->
            let o = ExpandoObject()
            o |> Dyn.set name value
            o |> Dyn.get name = value)

    [<Fact>]
    let ``?<- then ? returns the value on an Expando`` () =
        check (fun (NonNull (name: string)) (value: string) ->
            let o = ExpandoObject()
            (?<-) o name value
            let read: string = (?) o name
            read = value)

    [<Fact>]
    let ``tryGet and exists never throw and agree on an Expando`` () =
        check (fun (members: Map<NonNull<string>, int>) (NonNull (probe: string)) ->
            let o = ExpandoObject()
            for KeyValue (NonNull name, value) in members do
                o |> Dyn.set name value
            let found: int option = o |> Dyn.tryGet probe
            let expected = members |> Map.tryFind (NonNull probe)
            found = expected && (o |> Dyn.exists probe) = expected.IsSome)

    [<Fact>]
    let ``tryGet on a CLR object finds only its real member`` () =
        check (fun (NonNull (probe: string)) ->
            let o = Clr()
            let found: string option = o |> Dyn.tryGet probe
            if probe = "Name" then found = Some "Ada" else found = None)
