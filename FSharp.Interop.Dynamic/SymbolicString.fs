namespace FSharp.Interop.Dynamic

open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Reflection
module SymbolicString =

    let faux<'TTarget> : 'TTarget = failwith "don't call the faux instance, meant for quotations only!"
    
    
    type Name =
        static member Of([<ReflectedDefinition>] value:Expr<'T>) : string =
            let rec finalName value' =
                match value' with
                    | ValueWithName(_, _, name) -> name
                    | NewUnionCase(caseInfo, _) -> caseInfo.Name
                    | PropertyGet(_, propOrValInfo, _) -> propOrValInfo.Name
                    | FieldGet(_, fieldInfo) -> fieldInfo.Name
                    | Call(_, methInfo, _) -> methInfo.Name
                    | Lambda(_, expr) -> finalName expr
                    | Let(_,_, expr) -> finalName expr
                    | ________________________________ -> invalidArg "value" (sprintf "Couldn't figure out how to make '%A' a name" value)
            finalName value