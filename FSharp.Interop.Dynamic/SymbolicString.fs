namespace FSharp.Interop.Dynamic

open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Reflection
module SymbolicString =

    let sym<'TTarget> : 'TTarget = failwith "don't call the sym function, meant for quotations only!"
    
    type LeafInfo = 
        { ReturnType : System.Type
          Name : string
          DeclaringType: System.Type option
         }
    
    type Symbol =

        static member leafInfoOf([<ReflectedDefinition>] value:Expr<'T>) : LeafInfo =
                    let rec finalName value' =
                        match value' with
                            | ValueWithName(_, type', name) -> 
                                { ReturnType= type'; Name= name; DeclaringType = None }
                            | NewUnionCase(caseInfo, _) -> 
                                { ReturnType= caseInfo.DeclaringType
                                  Name= caseInfo.Name
                                  DeclaringType= Some <| caseInfo.DeclaringType }
                            | PropertyGet(_, propOrValInfo, _) ->
                                { ReturnType= propOrValInfo.PropertyType
                                  Name= propOrValInfo.Name
                                  DeclaringType= Some <| propOrValInfo.DeclaringType }
                            | FieldGet(_, fieldInfo) ->
                                { ReturnType= fieldInfo.FieldType
                                  Name= fieldInfo.Name
                                  DeclaringType= Some <| fieldInfo.DeclaringType }
                            | Call(_, methInfo, _) ->
                                { ReturnType= methInfo.ReturnType
                                  Name= methInfo.Name
                                  DeclaringType= Some <| methInfo.DeclaringType }
                            | Lambda(_, expr) -> finalName expr
                            | Let(_,_, expr) -> finalName expr
                            | ________________________________ -> invalidArg "value" (sprintf "Couldn't figure out how to make '%A' a name" value)
                    finalName value
                    
        static member nameOf([<ReflectedDefinition>] value:Expr<'T>) : string = (value |> Symbol.leafInfoOf).Name
              
        static member typeOf([<ReflectedDefinition>] value:Expr<'T>) : System.Type = (value |> Symbol.leafInfoOf).ReturnType
                     