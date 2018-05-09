
(**
# Examples of using Dynamic operator and Functions
*)
namespace Tests

open FSharp.Interop.Dynamic
open FSharp.Interop.Dynamic.Operators
(***hide***)
open NUnit.Framework
open System
open Dynamitey
open System.Dynamic
open System.Collections.Generic
open System.Xml.Linq
open System.Numerics
open Microsoft.CSharp.RuntimeBinder
open FsUnit
open System.Linq.Expressions

module Tests=

(***hide***)
    type TestEvent()=
        let event1 = new Event<EventHandler<EventArgs>, EventArgs>()
        [<CLIEvent>]
        member this.Event = event1.Publish
        member this.OnEvent(obj:Object, args:EventArgs)=
           event1.Trigger(obj,args)

(***hide***)
    type TestFuncs()=
        static member Plus3:Func<int,int> =
          Return<int>.Arguments<int>(fun x-> x + 3)
(***hide***)
    type DynamicOperatorMock()=
        inherit DynamicObject()
        override this.TryBinaryOperation(binder, arg, result) =
            result <- binder.Operation
            true
(***hide***)
    type DynamicWeirdFlakyIndexer()=
        inherit DynamicObject()
        let stuff = Dictionary<obj * obj, obj>()

        override this.TryGetIndex(binder, indexes, result) =
            result <- stuff.[(indexes.[0], indexes.[1])]
            true
        
        override this.TrySetIndex(binder, indexes, value) =
            stuff.Add((indexes.[0], indexes.[1]),value)
            true

    [<TestFixture>]
    type ``Basic Dynamic Operator Tests`` ()=

(**
Call a method with dlr (ideally you wouldn't know it was as a string).
*)
        [<Test>] member basic.``Call method off of an object dynamically`` ()=
                       "HelloWorld"?Substring(0,5) 
                            |> should equal "Hello"

(**
Set a property with dlr, Expando only responds to the dlr.
*)
        [<Test>] member basic.``Test Expando Set and Get`` ()=
                        let ex1 = ExpandoObject()
                        ex1?Test<-"Hi";
                        ex1?Test |> should equal "Hi"

(***hide***)
        [<Test>] member basic.``Test Direct Invoke`` ()=
                        !?Dynamic.Curry(Dyn.staticTarget<string>)?Format("Test {0} {1}") (1,2) |>
                            should equal "Test 1 2"

(***hide***)
        [<Test>] member basic.``Test Void Method`` ()=
                        let array = List<string>()
                        array?Add("1");
                        array.[0] |> should equal "1"

(***hide***)
        [<Test>] member basic.``Test SetAll`` ()=
                        let e1 = ExpandoObject()
                        !?Dynamic.InvokeSetAll (e1, [("One",1);("Two",2)])
                        e1?One |> should equal 1
                        e1?Two |> should equal 2

(***hide***)
        [<Test>] member basic.``Test Lambda methods`` ()=
                        let ex1 = DynamicObjects.Dictionary()
                        ex1?TestLam<- (fun x -> 42 + x)
                        ex1?TestLam2<- (fun x y -> y+ 42 + x)
                        ex1?TestDel<- TestFuncs.Plus3
                        ex1?TestLam(1) |> should equal 43
                        ex1?TestLam2(1, 2) |> should equal 45
                        ex1?TestDel(2) |> should equal 5

(***hide***)
        [<Test>] member basic.``Test FSharp Lambda 3 arg `` ()=
                        let dyn = (fun x y z -> x + y - z) :> obj
                        !?dyn (3,2,1) |> should equal 4

(***hide***)
        [<Test>] member basic.``Test FSharp Lambda 4 arg`` ()=
                        let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
                        !?dyn (3, 2, 1, 5) |> should equal -1

(***hide***)
        [<Test>] member basic.``Test FSharp Lambda 5 arg`` ()=
                        let unknownfunc = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
                        !?unknownfunc (3, 2, 1, 5, 9) |> should equal 8

(***hide***)
        [<Test>] member basic.``Test Events`` ()=
                        let pocoObj = TestEvent()
                        let refBool = ref false
                        let myevent = EventHandler<EventArgs>(fun obj arg -> (refBool := true))

                        //Add event dynamically
                        Dyn.addAssignMember pocoObj "Event" myevent
                        pocoObj.OnEvent(null,null)
                        !refBool |> should equal true

                        //Remove event dynamically
                        refBool :=false
                        Dyn.subtractAssignMember pocoObj "Event" myevent
                        !refBool |> should equal false

(**
`!?` will invoke without a name, dynamic function or the like. 
`Dyn.namedArg` allows you to wrap your arguments with names as part of the invocation.
*)
        [<Test>] member basic.``Test NamedArgs`` ()=
                        let buildObj = !?Build<ExpandoObject>.NewObject (
                                                                            Dyn.namedArg "One" 1,
                                                                            Dyn.namedArg "Two" 2
                                                                        )
                        buildObj?One |> should equal 1
                        buildObj?Two |> should equal 2

(**
Use the dlr to call the explict operator with a reflected type
*)
        [<Test>] member basic.``Test dynamic Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        ele |> Dyn.explicitConvertTo typeof<Int32> |> should equal 50

(**
Use the dlr to call the implict operato rwith a reflected type
*)
        [<Test>] member basic.``Test dynamic Implicit Conversion`` ()=
                        let ele = 50
                        ele |> Dyn.implicitConvertTo typeof<decimal> |> should equal (decimal 50)

(**
Use the dlr to call the explict operator with inferred type from usage
*)
        [<Test>] member basic.``Test Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        let elet:int = Dyn.explicitConvert ele
                        elet |> should equal 50

(**
Use the dlr to call the implicit operator with inferred type from usage
*)
        [<Test>] member basic.``Test Implicit Conversion`` ()=
                        let ele = 50
                        ele |> Dyn.implicitConvert |> should equal (decimal 50)


(***hide***)
        [<Test>] member basic.``Test Implicit Conversion Fail`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        (fun () -> Dyn.implicitConvert(ele) = 50 |> ignore) 
                            |> should throw typeof<RuntimeBinderException>

(***hide***)
        [<Test>] member basic.``Test Basic indexer `` ()=
                        let archive:obj = upcast DynamicWeirdFlakyIndexer()

                        Dyn.setIndex archive [1; 5] "A"
                        Dyn.setIndex archive ["Hello"; "World" ] "B"
                        Dyn.setIndex archive [box 1; box "World" ] "C"

                        Dyn.getIndex archive [1; 5] |> should equal "A"
                        Dyn.getIndex archive ["Hello"; "World" ] |> should equal "B"
                        Dyn.getIndex archive [box 1; box "World"] |> should equal "C"
                        
(***hide***)
        [<Test>] member basic.``Basic Operator Mock Tests`` ()=
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
         [<Test>] member basic.``Basic Operator Op Tests`` ()=
                       

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
