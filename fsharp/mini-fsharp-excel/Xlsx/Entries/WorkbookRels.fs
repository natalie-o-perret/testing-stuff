[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.Entries.WorkbookRels

open FSharpPlus

open MiniExcelFSharp.Xlsx
open MiniExcelFSharp.Helpers
open MiniExcelFSharp.Xlsx.ReadOnlyWrapper
open MiniExcelFSharp.Xlsx.Entries.Constants


let getAll zipArchive =
    seq {
        use stream = ZipArchive.OpenEntry zipArchive WorkbookRelsNames.Entry
        use xmlReader = SimpleXmlReader.ofStream stream

        if xmlReader.ReadToFollowing(WorkbookRelsNames.Elements.Relationships) then
            while xmlReader.ReadToFollowing(WorkbookRelsNames.Elements.Relationship) do
                yield xmlReader.GetAttribute(WorkbookRelsNames.Attributes.Id),
                      xmlReader.GetAttribute(WorkbookRelsNames.Attributes.Target)
        else
            invalidOp $"{WorkbookRelsNames.Elements.Relationships} XML element not found in {WorkbookRelsNames.Entry}"
    }
    |> ReadOnlyDict.ofTuples

let getWorksheets zipArchive =
    getAll zipArchive
    |> ReadOnlyDict.filter (fun _ -> String.startsWith @$"{WorkbookRelsNames.Prefixes.Worksheets}/")
