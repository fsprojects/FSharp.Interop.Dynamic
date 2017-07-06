// Learn more about F# at http://fsharp.net

namespace Tests


open NUnit.Framework
open System
open Dynamitey
open System.Dynamic
open System.Collections.Generic
open System.Xml.Linq
open System.Numerics
open Microsoft.CSharp.RuntimeBinder
open FSharp.Interop.Dynamic
open FsUnit

module Tests=

    type TestEvent()=
        let event1 = new Event<EventHandler<EventArgs>, EventArgs>()
        [<CLIEvent>]
        member this.Event = event1.Publish
        member this.OnEvent(obj:Object, args:EventArgs)=
           event1.Trigger(obj,args)

    type TestFuncs()=
        static member Plus3:Func<int,int> =
          Return<int>.Arguments<int>(fun x-> x + 3)


    [<TestFixture>]
    type ``Basic Dynamic Operator Tests`` ()=


        [<Test>] member basic.``Call method off of an object dynamically`` ()=
                       "HelloWorld"?Substring(0,5) |> should equal "Hello"


        [<Test>] member basic.``Test Expando Set and Get`` ()=
                        let ex1 = ExpandoObject()
                        ex1?Test<-"Hi";
                        ex1?Test |> should equal "Hi"


        [<Test>] member basic.``Test Direct Invoke`` ()=
                        !?Dynamic.Curry(Dyn.staticContext(typeof<string>))?Format("Test {0} {1}") (1,2) |>
                            should equal "Test 1 2"


        [<Test>] member basic.``Test Void Method`` ()=
                        let array = List<string>()
                        array?Add("1");
                        array.[0] |> should equal "1"


        [<Test>] member basic.``Test SetAll`` ()=
                        let e1 = ExpandoObject()
                        !?Dynamic.InvokeSetAll (e1, [("One",1);("Two",2)])
                        e1?One |> should equal 1
                        e1?Two |> should equal 2


        [<Test>] member basic.``Test Lambda methods`` ()=
                        let ex1 = DynamicObjects.Dictionary()
                        ex1?TestLam<- (fun x -> 42 + x)
                        ex1?TestLam2<- (fun x y -> y+ 42 + x)
                        ex1?TestDel<- TestFuncs.Plus3
                        ex1?TestLam(1) |> should equal 43
                        ex1?TestLam2(1, 2) |> should equal 45
                        ex1?TestDel(2) |> should equal 5


        [<Test>] member basic.``Test FSharp Lambda 3 arg `` ()=
                        let dyn = (fun x y z -> x + y - z) :> obj
                        !?dyn (3,2,1) |> should equal 4


        [<Test>] member basic.``Test FSharp Lambda 4 arg`` ()=
                        let dyn = (fun x y z bbq -> x + y - z - bbq) :> obj  in
                        !?dyn (3, 2, 1, 5) |> should equal -1


        [<Test>] member basic.``Test FSharp Lambda 5 arg`` ()=
                        let unknownfunc = (fun x y z bbq etc -> x + y - z - bbq + etc) :> obj in
                        !?unknownfunc (3, 2, 1, 5, 9) |> should equal 8


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


        [<Test>] member basic.``Test NamedArgs`` ()=
                        let buildObj = !?Build<ExpandoObject>.NewObject (
                                                                            Dyn.namedArg "One" 1,
                                                                            Dyn.namedArg "Two" 2
                                                                        )
                        buildObj?One |> should equal 1
                        buildObj?Two |> should equal 2


        [<Test>] member basic.``Test dynamic Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        ele |> Dyn.explicitConvertTo typeof<Int32> |> should equal 50


        [<Test>] member basic.``Test dynamic Implicit Conversion`` ()=
                        let ele = 50
                        ele |> Dyn.implicitConvertTo typeof<decimal> |> should equal (decimal 50)

        [<Test>] member basic.``Test Explicit Conversion`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        let elet:int = Dyn.explicitConvert ele
                        elet |> should equal 50


        [<Test>] member basic.``Test Implicit Conversion`` ()=
                        let ele = 50
                        ele |> Dyn.implicitConvert |> should equal (decimal 50)


        [<Test>] member basic.``Test Implicit Conversion Fail`` ()=
                        let ele = XElement(XName.Get("Test"),"50")
                        (fun () -> Dyn.implicitConvert(ele) = 50 |> ignore) |> should throw typeof<RuntimeBinderException>
