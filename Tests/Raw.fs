namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
open FSharp.Interop.Dynamic.SymbolicString

open Xunit
open FsUnit.Xunit
open System.Linq

module Raw =

   [<Fact>] 
    let ``Name of Call`` ()=
        Name.Of(faux<string>.Substring(0,0)) |> should equal "Substring"
  
    [<Fact>]
    let ``Name of unapplied method`` ()=
        Name.Of(faux<string>.Substring) |> should equal "Substring"
    
    [<Fact>]
    let ``Name of get`` ()=
        Name.Of(faux<string>.Length) |> should equal "Length"
    
    [<Fact>]
    let ``Name of end of chain`` ()=
        Name.Of(faux<string>.Length.CompareTo) |> should equal "CompareTo"
    
    [<Fact>]
    let ``Name of End of chain overload`` ()=
        Name.Of(faux<string>.Length.ToString(faux<string>)) |> should equal "ToString"

    [<Fact>]
    let ``Name of End of chain mix and match extension`` ()=
        Name.Of(faux<string>.Length.ToString(faux<string>).Length.ToString().Any) |> should equal "Any"
 
    [<Fact>]
    let ``Name of var`` ()=
        let x = 0
        Name.Of(x) |> should equal "x"

    [<Fact>]
    let ``Name of static generic member`` ()=
        Name.Of(Enumerable.Empty) |> should equal "Empty"
    
    [<Fact>]
    let ``Name of DU`` ()=
        let x = 0
        Name.Of(Option.Some) |> should equal "Some"

    [<Fact>]
    let ``Call method off of an object dynamically`` ()=
        let name = Name.Of(faux<string>.Substring(0,0))
        let actual = "HelloWorld" |> Dyn.invokeMember name (0,5) 
        actual |> should equal "Hello"