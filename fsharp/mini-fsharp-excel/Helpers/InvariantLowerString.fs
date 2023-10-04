[<RequireQualifiedAccess>]
module MiniExcelFSharp.Helpers.InvariantLowerString

open FSharpPlus


let isNotNullOrEmpty = String.toLower >> String.isNotNullOrEmpty

let isNotNullOrEmptyOr values source =
    let lowerInvariant = String.toLower source
    String.isNotNullOrEmpty lowerInvariant &&
    Seq.forall ((<>) source) values
