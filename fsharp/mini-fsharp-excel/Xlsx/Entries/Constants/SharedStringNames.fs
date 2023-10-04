[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.Entries.Constants.SharedStringNames


[<Literal>]
let Entry = "xl/sharedStrings.xml"

[<RequireQualifiedAccess>]
module Attributes =

    [<Literal>]
    let PreservedSpace = "space"

    [<Literal>]
    let PreserveSpaceValue = "preserve"

[<RequireQualifiedAccess>]
module Elements =
    [<Literal>]
    let SharedStringTable = "sst"

    [<Literal>]
    let StringItem = "si"

    [<Literal>]
    let Text = "t"

    [<Literal>]
    let RichTextRun = "r"

    [<Literal>]
    let RichTextRunProperties = "rPr"
