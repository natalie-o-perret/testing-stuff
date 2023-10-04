[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.Entries.Constants.WorksheetNames


let getEntry (n: uint16) = $"xl/worksheets/sheet%d{n}.xml"

[<RequireQualifiedAccess>]
module Elements =
    [<Literal>]
    let Worksheet = "worksheet"

    [<Literal>]
    let SheetData = "sheetData"

    [<Literal>]
    let Row = "row"

    [<Literal>]
    let Cell = "c"

    [<Literal>]
    let CellValue = "v"

    [<Literal>]
    let CellFormula = "f"

    [<Literal>]
    let CellStyle = "s"

    [<Literal>]
    let CellInlineString = "is"

    [<Literal>]
    let CellInlineStringText = "t"

    [<Literal>]
    let CellInlineStringRichTextRun = "r"

    [<Literal>]
    let CellInlineStringRichTextRunProperties = "rPr"

[<RequireQualifiedAccess>]
module Attributes =
    [<Literal>]
    let Dimension = "dimension"

    [<Literal>]
    let Reference = "ref"

    [<Literal>]
    let RowIndex = "r"

    [<Literal>]
    let ColumnIndex = "c"

    [<Literal>]
    let CellType = "t"

    [<Literal>]
    let CellStyle = "s"

    [<Literal>]
    let CellFormula = "f"

    [<Literal>]
    let CellFormulaType = "t"

    [<Literal>]
    let CellFormulaSharedIndex = "si"

    [<Literal>]
    let CellFormulaSharedRef = "ref"

    [<Literal>]
    let CellFormulaSharedCount = "ca"

    [<Literal>]
    let CellFormulaSharedGroup = "gs"

    [<Literal>]
    let CellFormulaSharedOldRef = "oldref"

    [<Literal>]
    let CellFormulaSharedOldCount = "oldca"

    [<Literal>]
    let CellFormulaSharedOldGroup = "oldgs"

    [<Literal>]
    let CellFormulaSharedNewRef = "newref"

    [<Literal>]
    let CellFormulaSharedNewCount = "newca"

    [<Literal>]
    let CellFormulaSharedNewGroup = "newgs"

    [<Literal>]
    let CellFormulaSharedDeleted = "deleted"

    [<Literal>]
    let CellFormulaShared = "shared"
