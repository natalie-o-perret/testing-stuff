[<RequireQualifiedAccess>]
module MiniExcelFSharp.Helpers.String

open System


let isNotNullOrEmpty  = String.IsNullOrWhiteSpace >> not
