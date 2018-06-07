namespace Tests

open FSharp.Interop.Dynamic
open Xunit
open FsUnit.Xunit
open Tests
open System
open System.Dynamic
 
#nowarn "44"

module Obsolete =

    [<Fact>]
    let ``Test Events`` ()=
        let pocoObj = TestEvent()
        let refBool = ref false
        let myevent = EventHandler<EventArgs>(fun _ _ -> (refBool := true))

        //Add event dynamically
        Dyn.addAssignMember pocoObj "Event" myevent
        pocoObj.OnEvent(null,null)
        !refBool |> should equal true

        //Remove event dynamically
        refBool :=false
        Dyn.subtractAssignMember pocoObj "Event" myevent
        !refBool |> should equal false

    [<Fact>]
    let ``Test Basic indexer`` ()=
        let archive:obj = upcast DynamicWeirdFlakyIndexer()

        Dyn.setIndex archive [1; 5] "A"
        Dyn.setIndex archive ["Hello"; "World" ] "B"
        Dyn.setIndex archive [box 1; box "World" ] "C"

        Dyn.getIndex archive  [1; 5] |> should equal "A"
        Dyn.getIndex archive  ["Hello"; "World" ] |> should equal "B"
        Dyn.getIndex archive  [box 1; box "World"] |> should equal "C"

    [<Fact>]
    let ``Call method off of an object dynamically with variable`` ()=
       let method = "Substring"
       Dyn.invoke "HelloWorld" (Some method) (0,5) 
            |> should equal "Hello"
