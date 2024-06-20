open System
open System.IO
open System.Text.RegularExpressions
module Patterns =
    // Active pattern is a function that takes value in some sort of union of possible values
    // it can be used in pattern matching & destructuring and all kinds of good stuff
    
    // Partial active pattern : case-insensitive matching. Matches a pattern or wildcard
    let (|CI|_|) expected actual =
        if String.Equals(expected, actual, StringComparison.CurrentCultureIgnoreCase) then
            Some ()  // Active patterns can return content! If you just want to match or not match, return "unit" ()
        else None
    
    // Exhaustive Active Pattern
    let (|FileContent|FileException|) path =
        if File.Exists path then
            try
               FileContent (File.ReadAllText path)
            with
            | :? UnauthorizedAccessException -> FileException "Unauthorized Access"
        else
            FileException "No such file exists"
        
    let (|Regexp|_|) pattern input =
        let matched = Regex.Match(input,pattern)
        if matched.Success then Some ()
        else None
    let (|Regexp1|_|) pattern input =
        let matched = Regex.Match(input,pattern)
        if matched.Success then Some matched.Groups[1].Value
        else None
    let (|IsJson|_|) =
        function
        | Regexp ("\w+\.json") -> Some()
        | _ -> None
    let (|FileExtension|_|) =
        function
        | Regexp1 ("\w+\.(\w+)") ext -> Some ext
        | _ -> None
    
    let (|Json|_|) (input: string) =
        try (System.Text.Json.JsonDocument.Parse(input).RootElement) |> Some
        with
        | :? System.Text.Json.JsonException -> None
        
    


[<EntryPoint>]
let main argv =
    match List.ofArray argv with
    | (Patterns.FileExtension "json" & Patterns.FileContent content) :: rest -> printfn $"File content: {content}"    
    | Patterns.FileException exn :: rest -> printfn $"File error: {exn}"
    | Patterns.FileExtension other :: rest -> printfn $"File extension is not json: {other}"
    | _ -> printfn "Please provide a file"
    0