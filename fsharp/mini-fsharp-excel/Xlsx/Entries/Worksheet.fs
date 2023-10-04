[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.Entries.Worksheet

open System.Xml

open FSharpPlus

open MiniExcelFSharp.Xlsx
open MiniExcelFSharp.Helpers
open MiniExcelFSharp.Xlsx.ReadOnlyWrapper


[<Literal>]
let WorksheetParentEntry = "xl"

[<Literal>]
let RowXmlElementName = "row"

[<Literal>]
let RowCellXmlElementName = "c"

[<Literal>]
let RowCellValueXmlElementName = "v"

[<Literal>]
let RowCellFormulaXmlElementName = "f"

[<Literal>]
let RowCellFormulaXmlAlwaysCalculateArrayAttributeName = "aca"

[<Literal>]
let RowNoXmlAttributeName = "r"

[<Literal>]
let RowCellCoordinateXmlAttributeName = "r"

[<Literal>]
let RowCellValueTypeXmlAttributeName = "t"

[<Literal>]
let RowCellStyleXmlAttributeName = "s"

[<Literal>]
let RowCellSharedStringValueType = "s"


let validCellDescendantLocalNames =
    set [ RowCellFormulaXmlElementName; RowCellValueXmlElementName ]

let inline private readCellFormula (xmlReader: XmlReader) =
    // The "aca" attribute in the context of an Excel Worksheet XML refers to "Always Calculate Array."
    // This attribute is used within the <f> (formula) element when the formula is an array formula.
    // When the aca attribute is set to "1" or "true," Excel will always recalculate the array formula,
    // even if the calculation mode is set to manual.
    // By default, the aca attribute is set to "0" or "false,"
    // which means the array formula will be calculated according to the workbook's calculation settings.

    let aca =
        xmlReader.GetAttribute(RowCellFormulaXmlAlwaysCalculateArrayAttributeName)
        |> InvariantLowerString.isNotNullOrEmptyOr [ "0"; "false" ]

    let formula = xmlReader.ReadInnerXml()
    aca, formula

let inline private readCell (xmlReader: XmlReader) rowNoString =
    let cellCoordinates = xmlReader.GetAttribute(RowCellCoordinateXmlAttributeName)

    let column =
        cellCoordinates.[0 .. ((String.length cellCoordinates - String.length rowNoString) - 1)]

    // The "t" attribute for the cell type in the <c> (cell) element of a Worksheet XML is not mandatory.
    // If the "t" attribute is not present, Excel assumes the cell type is "n" (Number).
    // The "t" attribute is used to specify the data type of the cell value and can have one of the following values:
    // - "s": Shared String (when the cell contains a string that is stored in the shared strings table)
    // - "str" or "inlineStr": String (when the cell contains a string that is not stored in the shared strings table)
    // -- "str": it is used for formula cells with string results.
    //    In this case, the <c> element contains a <f> element to store the formula and a <v> element to store the string result.
    //    The str cell type is less common in regular data cells.
    // -- "inlineStr": used when the string is unique and not expected to be repeated across multiple cells.
    //    The "inlineStr" cell type is more commonly used in Excel files created by third-party applications or tools,
    //    as it does not require a shared strings table.
    // - "n": Number (when the cell contains a numeric value)
    // - "b": Boolean (when the cell contains a boolean value)
    // - "e": Error (when the cell contains an error value, such as "#DIV/0!")
    let cellType = xmlReader.GetAttribute(RowCellValueTypeXmlAttributeName)
                   |> Option.ofObj

    // The "s" attribute is not mandatory in the <c> (cell) element of a Worksheet XML.
    // If the "s" attribute is not present, it means that the cell does not have a specific style applied,
    // and it will use the default style defined in the "styles.xml" file.
    // The default style is usually the first <xf> element in the <cellXfs> collection in the "styles.xml" file,
    // with an index of "0". If the s attribute is not specified, Excel assumes the cell uses the default style.
    let cellStyle = xmlReader.GetAttribute(RowCellStyleXmlAttributeName)
                    |> Option.ofObj
                    |> Option.map int32

    match SimpleXmlReader.readToFirstDescendantWithAnyOf xmlReader validCellDescendantLocalNames with
    | Some RowCellFormulaXmlElementName ->
        // The <f> (formula) element can be a child of a <c> (cell) element regardless of the cell type specified by the t attribute.
        // However, it's more common for certain cell types to contain formulas, such as:
        // - numeric cells (t="n") or,
        // - string cells with formula results (t="str").
        // When a cell contains a formula, the <f> element is used to store the formula expression,
        // and the <v> element is used to store the calculated result of the formula.
        // Depending on the result of the formula, the cell type (t attribute) may need to be adjusted accordingly.

        let aca, cellFormula = readCellFormula xmlReader

        if xmlReader.Name = RowCellValueXmlElementName then
            { Column = column
              Type = cellType
              Style = cellStyle
              Formula = Some(aca, cellFormula)
              Value = Some(xmlReader.ReadInnerXml()) }
        else
            { Column = column
              Type = cellType
              Style = cellStyle
              Formula = Some(aca, cellFormula)
              Value = None }
    | Some RowCellValueXmlElementName ->
        { Column = column
          Type = cellType
          Style = cellStyle
          Formula = None
          Value = Some(xmlReader.ReadInnerXml()) }
    | _ ->
        { Column = column
          Type = cellType
          Style = cellStyle
          Formula = None
          Value = None }

let inline getRows relPath zipArchive =
    seq {
        let fullRelPath = $"%s{WorksheetParentEntry}/%s{relPath}"
        use stream = ZipArchive.OpenEntry zipArchive fullRelPath
        use xmlReader = SimpleXmlReader.ofStream stream

        while xmlReader.ReadToFollowing(RowXmlElementName) do
            let rowNoString = xmlReader.GetAttribute(RowNoXmlAttributeName)

            let cells =
                ReadOnlyArray
                    [| if xmlReader.ReadToDescendant(RowCellXmlElementName) then
                           yield readCell xmlReader rowNoString
                       while xmlReader.ReadToNextSibling(RowCellXmlElementName) do
                           yield readCell xmlReader rowNoString |]

            yield
                {| No = int32 rowNoString
                   Cells = cells |}
    }
