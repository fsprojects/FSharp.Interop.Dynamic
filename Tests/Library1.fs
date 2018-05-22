
(**
# Examples of using Dynamic operator and Functions
*)
namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
(***hide***)
open Xunit
open System
open Dynamitey
open System.Dynamic
open System.Collections.Generic
open System.Xml.Linq
open System.Numerics
open Microsoft.CSharp.RuntimeBinder
open FsUnit.Xunit
open System.Linq.Expressions

module Tests =

(***hide***)
    type TestEvent()=
        let event1 = new Event<EventHandler<EventArgs>, EventArgs>()
        [<CLIEvent>]
        member __.Event = event1.Publish
        member __.OnEvent(obj:Object, args:EventArgs)=
           event1.Trigger(obj,args)

(***hide***)
    type TestFuncs()=
        static member Plus3:Func<int,int> =
          Return<int>.Arguments<int>(fun x-> x + 3)
(***hide***)
    type DynamicOperatorMock()=
        inherit DynamicObject()
        override __.TryBinaryOperation(binder, arg, result) =
            result <- binder.Operation
            true
(***hide***)
    type DynamicWeirdFlakyIndexer()=
        inherit DynamicObject()
        let stuff = Dictionary<obj * obj, obj>()

        override __.TryGetIndex(_, indexes, result) =
            result <- stuff.[(indexes.[0], indexes.[1])]
            true
        
        override __.TrySetIndex(_, indexes, value) =
            stuff.Add((indexes.[0], indexes.[1]),value)
            true


    (**
    Call a method with dlr (ideally you wouldn't know it was as a string).
    *)
    [<Fact>]
    let ``Call method off of an object dynamically`` ()=
        "HelloWorld"?Substring(0,5) 
        |> should equal "Hello"
(**
Call a method with a variable (ideally you wouldn't know it was as a string).
*)
    [<Fact>]
    let ``Call method off of an object dynamically with variable`` ()=
       let method = "Substring"
       "HelloWorld"?(method)(0,5) 
            |> should equal "Hello"
(**
Set a property with dlr, Expando only responds to the dlr.
*)
    [<Fact>]
    let ``Test Expando Set and Get`` ()=
        let ex1 = ExpandoObject()
        ex1?Test<-"Hi";
        ex1?Test |> should equal "Hi"

(***hide***)
    [<Fact>]
    let ``Test Direct Invoke`` ()=
        !?Dynamic.Curry(Dyn.staticTarget<string>)?Format("Test {0} {1}") (1,2) |>
            should equal "Test 1 2"

(***hide***)
    [<Fact>]
    let ``Test Void Method`` ()=
        let array = List<string>()
        array?Add("1");
        array.[0] |> should equal "1"

(***hide***)
    [<Fact>]
    let ``Test SetAll`` ()=
        let e1 = ExpandoObject()
        !?Dynamic.InvokeSetAll (e1, [("One",1);("Two",2)])
        e1?One |> should equal 1
        e1?Two |> should equal 2

(***hide***)
    [<Fact>]
    let ``Test Lambda methods`` ()=
        let ex1 = DynamicObjects.Dictionary()
        ex1?TestLam<- (fun x -> 42 + x)
        ex1?TestLam2<- (fun (x,y) -> y+ 42 + x)
        ex1?TestDel<- TestFuncs.Plus3
        ex1?TestLam(1) |> should equal 43
        ex1?TestLam2(1,2) |> should equal 45
        ex1?TestDel(2) |> should equal 5

(***hide***)
    [<Fact>]
    let ``Test FSharp Lambda Tuple arg`` ()=
        let dyn = (fun (x,y) z -> x + y - z) :> obj
        let x:int = !?dyn (3,2) 1
        x |> should equal 4

    [<Fact>]
    let ``Test FSharp Lambda 2 arg`` ()=
        let dyn = (( + )) :> obj
        let x = !?dyn 3 2
        x |> should equal 5
    [<Fact>]
    let ``Test FSharp Lambda 3 arg not tupled`` ()=
        let dyn = (fun x y z -> x + y - z) :> obj
        let x = !?dyn 3 2 1
        x |> should equal 4
(***hide***)
    [<Fact>]
    let ``Test FSharp Lambda 4 arg`` ()=
        let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
        let x = !?dyn 3 2 1 5 
        x |> should equal -1

(***hide***)
    [<Fact>]
    let ``Test FSharp Lambda 5 arg`` ()=
        let unknownfunc = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
        let go = !?unknownfunc
        let x = go 3 2 1 5 9 
        x |> should equal 8

(***hide***)
    [<Fact>]
    let ``Test Events`` ()=
        let pocoObj = TestEvent()
        let refBool = ref false
        let myevent = EventHandler<EventArgs>(fun _ _ -> (refBool := true))

        //Add event dynamically
        pocoObj |> Dyn.memberAddAssign "Event" myevent
        pocoObj.OnEvent(null,null)
        !refBool |> should equal true

        //Remove event dynamically
        refBool :=false
        pocoObj |> Dyn.memberSubtractAssign "Event" myevent
        !refBool |> should equal false

(**
`!?` will invoke without a name, dynamic function or the like. 
`Dyn.namedArg` allows you to wrap your arguments with names as part of the invocation.
*)
    [<Fact>]
    let ``Test NamedArgs`` ()=
        let buildObj = !?Build<ExpandoObject>.NewObject (
                                                            Dyn.namedArg "One" 1,
                                                            Dyn.namedArg "Two" 2
                                                        )
        buildObj?One |> should equal 1
        buildObj?Two |> should equal 2

(**
Use the dlr to call the explict operator with a reflected type
*)
    [<Fact>]
    let ``Test dynamic Explicit Conversion`` ()=
        let ele = XElement(XName.Get("Test"),"50")
        ele |> Dyn.explicitConvertTo typeof<Int32> |> should equal 50

(**
Use the dlr to call the implict operato rwith a reflected type
*)
    [<Fact>]
    let ``Test dynamic Implicit Conversion`` ()=
        let ele = 50
        let actual = ele |> Dyn.implicitConvertTo typeof<decimal>
        actual |> should equal 50M

(**
Use the dlr to call the explict operator with inferred type from usage
*)
    [<Fact>]
    let ``Test Explicit Conversion`` ()=
        let ele = XElement(XName.Get("Test"),"50")
        let elet:int = Dyn.explicitConvert ele
        elet |> should equal 50

(**
Use the dlr to call the implicit operator with inferred type from usage
*)
    [<Fact>]
    let ``Test Implicit Conversion`` ()=
        let ele = 50
        let actual:decimal = ele |> Dyn.implicitConvert
        actual |> should equal 50m


(***hide***)
    [<Fact>]
    let ``Test Implicit Conversion Fail`` ()=
        let ele = XElement(XName.Get("Test"),"50")
        (fun () -> Dyn.implicitConvert(ele) = 50 |> ignore) 
            |> should throw typeof<RuntimeBinderException>

(***hide***)
    [<Fact>]
    let ``Test Basic indexer`` ()=
        let archive:obj = upcast DynamicWeirdFlakyIndexer()

        archive |> Dyn.setIndexer  [1; 5] "A"
        archive |> Dyn.setIndexer ["Hello"; "World" ] "B"
        archive |> Dyn.setIndexer  [box 1; box "World" ] "C"

        archive |> Dyn.getIndexer  [1; 5] |> should equal "A"
        archive |> Dyn.getIndexer  ["Hello"; "World" ] |> should equal "B"
        archive |> Dyn.getIndexer  [box 1; box "World"] |> should equal "C"
                        
(***hide***)
    [<Fact>]
    let ``Basic Operator Mock Tests`` ()=
        let left:obj = upcast DynamicOperatorMock()
        let dummy = Object()

        left ?%? dummy |> should equal ExpressionType.Modulo
        left ?*? dummy |> should equal ExpressionType.Multiply
        left ?+? dummy |> should equal ExpressionType.Add
        left ?-? dummy |> should equal ExpressionType.Subtract
        left ?/? dummy |> should equal ExpressionType.Divide
        left ?&&&? dummy |> should equal ExpressionType.And
        left ?|||? dummy |> should equal ExpressionType.Or
        left ?^^^? dummy |> should equal ExpressionType.ExclusiveOr
        left ?<<<? dummy |> should equal ExpressionType.LeftShift
        left ?>>>? dummy |> should equal ExpressionType.RightShift
(**
Use operators dynamically (better without knowing the types).
*)
    [<Fact>]
    let ``Basic Operator Op Tests`` ()=
        65 ?%? 10 |> should equal 5
        5 ?*? 4 |> should equal 20
        5 ?+? 4 |> should equal 9
        5 ?-? 3 |> should equal 2
        15 ?/? 5 |> should equal 3
        
        5 ?&&&? 3 |> should equal 1
        5 ?|||? 3 |> should equal 7
        5 ?^^^? 3 |> should equal 6
        23 ?<<<? 2 |> should equal 92
        (-105) ?>>>? 1 |> should equal (-53)
        
        10 ?<=? 5 |> should equal false
        5 ?<=? 10 |> should equal true
        10 ?<=? 10 |> should equal true
        
        10 ?>=? 5 |> should equal true
        5 ?>=? 10 |> should equal false
        10 ?>=? 10 |> should equal true
        
        10 ?<? 5 |> should equal false
        5 ?<? 10 |> should equal true
        10 ?<? 10 |> should equal false
        
        10 ?>? 5 |> should equal true
        5 ?>? 10 |> should equal false
        10 ?>? 10 |> should equal false
        
        10 ?<>? 5 |> should equal true
        10 ?<>? 10 |> should equal false
        
        10 ?=? 5 |> should equal false
        10 ?=? 10 |> should equal true
