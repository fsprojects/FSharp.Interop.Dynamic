namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
open FSharp.Interop.Dynamic.SymbolicString

open Xunit
open FsUnit.Xunit
open System.Linq
open System
open System.Collections.Generic
open System.Dynamic
open Dynamitey


type FieldObj (i:int) as this=
    [<DefaultValue>]
    val mutable Field : int
    do 
      this.Field<- i

module Raw =
    open System.Collections

    [<Fact>] 
    let ``Name of Field`` ()=
        let fieldName = Symbol.nameOf(sym<FieldObj>.Field)
        fieldName |> should equal "Field"
        let target = FieldObj(7)
        target |> Dyn.get fieldName |> should equal 7


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
        
    [<Fact>]
    let ``implict convert null`` () =
        let actual:string = (null |> Dyn.implicitConvertTo typeof<string>)
        actual |> should equal null
        
    [<Fact>]
    let ``implict convert null option`` () =
        let actual:string option = (null |> Dyn.implicitConvertTo typeof<string option>)
        actual |> should equal None 
    [<Fact>]
    let ``implict convert null fail`` () =
        let actual ():int = null |> Dyn.implicitConvertTo typeof<int>
        actual >> ignore |> shouldFail 
        
    [<Fact>]
    let ``implict convert null fail fsharp type`` () =
        let actual ():Calling = (null |> Dyn.implicitConvertTo typeof<Calling>)
        actual >> ignore |> shouldFail 
       
    [<Fact>]
    let ``set dynamic chain`` () =
        let expected = "1";
        let target = ExpandoObject();
        let target2 = ExpandoObject();
        let target3 = ExpandoObject();
        target?Test <- target2;
        target2?Test2 <- target3;
        
        
        target |> Dyn.setChain ["Test"; "Test2"; "Test3"] expected;
        
        target?Test?Test2?Test3 |> should equal expected
    
    [<Fact>]
    let ``set dynamic get chain`` () =
        let expected = "1";
        let target = ExpandoObject();
        let target2 = ExpandoObject();
        let target3 = ExpandoObject();
        target?Test <- target2;
        target2?Test2 <- target3;
        target3?Test3 <- expected;
        
        let actual = target |> Dyn.getChain ["Test"; "Test2"; "Test3"];
        
        actual |> should equal expected 
        
    [<Fact>]
    let ``set dynamic get`` () =
        let expected = "A";
        let target = ExpandoObject();
        target?prop <- expected
        let actual = target |> Dyn.get "prop"
        actual |> should equal expected
     
    [<Fact>]
    let ``set dynamic set`` () =
        let expected = "A";
        let target = ExpandoObject();
        target |> Dyn.set "prop" expected
        let actual = target?prop
        actual |> should equal expected  
        
    [<Fact>]  
    let ``call static generic method`` () =
        let expected = Enumerable.Empty<int>()
        let target = Dyn.staticTarget<Enumerable>
        let actual: int seq = target |> Dyn.invokeGeneric "Empty" [typeof<int>] ()
        actual |> should equal expected  
        

    [<Fact>]  
    let ``call static generic method typeof`` () =
        let expected = Enumerable.Empty<int>()
        let target = Dyn.staticContext (typeof<Enumerable>)
        let actual: int seq = target |> Dyn.invokeGeneric "Empty" [typeof<int>] ()
        actual |> should equal expected  