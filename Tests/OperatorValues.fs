namespace Tests

open FSharp.Interop.Dynamic.Operators
open Xunit
open FsUnit.Xunit

module OperatorValues =

    [<Fact>]
    let ``dynamic add folds a list when passed as a function`` () =
        [1; 2; 3; 4] |> List.reduce (?+?) |> should equal 10

    [<Fact>]
    let ``dynamic multiply folds a list when passed as a function`` () =
        [2; 3; 4] |> List.reduce (?*?) |> should equal 24

    [<Fact>]
    let ``dynamic comparison filters a list when passed as a function`` () =
        let aboveTwo x = x ?>? 2
        [1; 2; 3; 4] |> List.filter aboveTwo |> should equal [3; 4]

    [<Fact>]
    let ``dynamic add on mixed numeric types uses the DLR conversion`` () =
        let sum: float = 5 ?+? 3.5
        sum |> should equal 8.5

    [<Fact>]
    let ``dynamic add concatenates strings`` () =
        let s: string = "Hello" ?+? " " ?+? "World"
        s |> should equal "Hello World"

    [<Fact>]
    let ``dynamic divide by zero raises the runtime divide-by-zero`` () =
        (fun () -> 1 ?/? 0 |> ignore)
        |> should throw typeof<System.DivideByZeroException>
