namespace FSharp.Interop.Dynamic

open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Reflection
/// Turns an F# quotation into a member name, or a LeafInfo with name, return type, and declaring type.
/// Use when you want the compiler to check the member rather than shipping a magic string.
module SymbolicString =

    /// Typed hole for quotations only. Do not evaluate; calling it throws.
    let sym<'TTarget> : 'TTarget = failwith "don't call the sym function, meant for quotations only!"
    
    /// Name, return type, and optional declaring type of a quoted leaf.
    type LeafInfo = 
        { /// Return type of the quoted leaf.
          ReturnType : System.Type
          /// Member, union-case, or value name of the quoted leaf.
          Name : string
          /// Declaring type when the leaf is a member, field, union case, or call; None for a named value.
          DeclaringType: System.Type option
         }
    
    /// Extracts name and type information from an F# quotation of a member, union case, or value.
    type Symbol =

        /// <summary>
        /// Walks a quotation to the leaf and returns its name, return type, and declaring type.
        /// Understands named values, union cases, property and field gets, method calls, and <c>fun</c> / <c>let</c> wrappers.
        /// Anything else (literals, tuples, conditionals) throws <c>ArgumentException</c>.
        /// </summary>
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
                    
        /// Member, union-case, or value name of the quoted leaf.
        static member nameOf([<ReflectedDefinition>] value:Expr<'T>) : string = (value |> Symbol.leafInfoOf).Name
              
        /// Return type of the quoted leaf.
        static member typeOf([<ReflectedDefinition>] value:Expr<'T>) : System.Type = (value |> Symbol.leafInfoOf).ReturnType
                     