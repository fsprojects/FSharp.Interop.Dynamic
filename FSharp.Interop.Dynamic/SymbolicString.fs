namespace FSharp.Interop.Dynamic

open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Reflection
module SymbolicString =

    let sym<'TTarget> : 'TTarget = failwith "don't call the sym function, meant for quotations only!"
    
    
    type Symbol =

        static member nameWithTypeOf([<ReflectedDefinition>] value:Expr<'T>) : string * System.Type =
                    let rec finalName value' =
                        match value' with
                            | ValueWithName(_, type', name) -> name, type'
                            | NewUnionCase(caseInfo, _) -> caseInfo.Name, caseInfo.DeclaringType
                            | PropertyGet(_, propOrValInfo, _) -> propOrValInfo.Name, propOrValInfo.PropertyType
                            | FieldGet(_, fieldInfo) -> fieldInfo.Name, fieldInfo.FieldType
                            | Call(_, methInfo, _) -> methInfo.Name, methInfo.ReturnType
                            | Lambda(_, expr) -> finalName expr
                            | Let(_,_, expr) -> finalName expr
                            | ________________________________ -> invalidArg "value" (sprintf "Couldn't figure out how to make '%A' a name" value)
                    finalName value
                    
        static member nameOf([<ReflectedDefinition>] value:Expr<'T>) : string = value |> Symbol.nameWithTypeOf |> fst
              
        static member typeOf([<ReflectedDefinition>] value:Expr<'T>) : System.Type = value |> Symbol.nameWithTypeOf |> snd
                     