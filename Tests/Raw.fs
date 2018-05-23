namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
open FSharp.Interop.Dynamic.SymbolicString

open Xunit
open FsUnit.Xunit
open System.Linq
open System
open System.Collections.Generic

module Raw =
    open System.Collections

    [<Fact>] 
    let ``Name of Call`` ()=
        Symbol.nameOf(sym<string>.Substring(0,0)) |> should equal "Substring"
        Symbol.typeOf(sym<string>.Substring(0,0)) |> should equal typeof<string>
  
    [<Fact>]
    let ``Name of unapplied method`` ()=
        Symbol.nameOf(sym<string>.Substring) |> should equal "Substring"
        Symbol.typeOf(sym<string>.Substring) |> should equal typeof<string>
    
    [<Fact>]
    let ``Name of get`` ()=
        Symbol.nameOf(sym<string>.Length) |> should equal "Length"
        Symbol.typeOf(sym<string>.Length) |> should equal typeof<int>
    
    [<Fact>]
    let ``Name of end of chain`` ()=
        Symbol.nameOf(sym<string>.Length.CompareTo) |> should equal "CompareTo"
        Symbol.typeOf(sym<string>.Length.CompareTo) |> should equal typeof<int>
    
    [<Fact>]
    let ``Name of End of chain overload`` ()=
        Symbol.nameOf(sym<string>.Length.ToString(sym<string>)) |> should equal "ToString"
        Symbol.typeOf(sym<string>.Length.ToString(sym<string>)) |> should equal typeof<string>

    [<Fact>]
    let ``Name of End of chain mix and match extension`` ()=
        Symbol.nameOf(sym<string>.Length.ToString(sym<string>).Length.ToString().Any) |> should equal "Any"
        Symbol.typeOf(sym<string>.Length.ToString(sym<string>).Length.ToString().Any) |> should equal typeof<bool>
    [<Fact>]
    let ``Name of var`` ()=
        let x = 0
        Symbol.nameOf(x) |> should equal "x"
        Symbol.typeOf(x) |> should equal typeof<int>

    [<Fact>]
    let ``Name of static generic member`` ()=
        Symbol.nameOf(Enumerable.Empty) |> should equal "Empty"
        Symbol.typeOf(Enumerable.Empty) |> should equal typeof<IEnumerable<_>>
    [<Fact>]
    let ``Name of DU`` ()=
        let x = 0
        Symbol.nameOf(Option.Some) |> should equal "Some"
        Symbol.typeOf(Option.Some) |> should equal typeof<Option<_>>
    [<Fact>]
    let ``Call method off of an object dynamically`` ()=
        let name = Symbol.nameOf(sym<string>.Substring(0,0))
        let actual = "HelloWorld" |> Dyn.invokeMember name (0,5) 
        actual |> should equal "Hello"