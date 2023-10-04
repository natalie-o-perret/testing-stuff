[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.Entries.SharedStrings

open System.Xml

open FSharpPlus

open MiniExcelFSharp.Xlsx
open MiniExcelFSharp.Helpers
open MiniExcelFSharp.Xlsx.ReadOnlyWrapper
open MiniExcelFSharp.Xlsx.Entries.Constants


type TextElement = { Value: string; PreserveSpace: bool }

type RichTextRun =
    { Text: TextElement }

type StringItem =
    | PlainText of TextElement
    | RichText of RichTextRun list

type SharedStringTable =
    { Counts: Count * UniqueCount
      StringItems: ReadOnlyArray<StringItem> }

let inline private getCountsCore (xmlReader: XmlReader) =
    let count       = xmlReader.GetAttribute("count")       |> parse : Count
    let uniqueCount = xmlReader.GetAttribute("uniqueCount") |> parse : UniqueCount
    count, uniqueCount

let inline private getRichTextRunTextsCore (xmlReader: XmlReader) =
    [| let stringItemDepth = xmlReader.Depth
       while stringItemDepth <= xmlReader.Depth && xmlReader.Read() do
           if xmlReader.NodeType = XmlNodeType.Element &&
              xmlReader.Name = SharedStringNames.Elements.Text then
                let preserveSpace = xmlReader.GetAttribute(SharedStringNames.Attributes.PreservedSpace)
                                      |> Option.ofObj
                                      |>> String.toLower
                                      |> option ((=) SharedStringNames.Attributes.PreserveSpaceValue) false
                yield { Value = ; PreserveSpace = preserveSpace }


                 |]

let inline private getStringItemsCore (xmlReader: XmlReader) =
    seq {
        let stringItemTypeNames = set [ SharedStringNames.Elements.Text
                                        SharedStringNames.Elements.RichTextRun ]
        while xmlReader.ReadToFollowing(SharedStringNames.Elements.StringItem) do
            match SimpleXmlReader.readToFirstDescendantWithAnyOf xmlReader stringItemTypeNames with
            | Some SharedStringNames.Elements.Text ->
                let value = xmlReader.ReadElementContentAsString()
                yield PlainText { Value = value; PreserveSpace = false }
            | Some SharedStringNames.Elements.RichTextRun ->
                yield PlainText getRichTextRunTextsCore xmlReader
                yield
            | _ ->
                ()
    }

let inline getCounts zipArchive =
    use stream = ZipArchive.OpenEntry zipArchive SharedStringNames.Entry
    use xmlReader = SimpleXmlReader.ofStream stream
    if xmlReader.ReadToDescendant(SharedStringNames.Elements.SharedStringTable) then
        getCountsCore xmlReader
    else
        invalidOp $"{SharedStringNames.Elements.SharedStringTable} XML element not found in {SharedStringNames.Entry}"

let inline getStringItems zipArchive =
    seq {
        use stream = ZipArchive.OpenEntry zipArchive SharedStringNames.Entry
        use xmlReader = SimpleXmlReader.ofStream stream
        if xmlReader.ReadToDescendant(SharedStringNames.Elements.SharedStringTable) then
            yield! getStringItemsCore xmlReader
        else
            invalidOp $"{SharedStringNames.Elements.SharedStringTable} XML element not found in {SharedStringNames.Entry}"
    }

let getSharedStringTable zipArchive =
    use stream = ZipArchive.OpenEntry zipArchive SharedStringNames.Entry
    use xmlReader = SimpleXmlReader.ofStream stream
    if xmlReader.ReadToDescendant(SharedStringNames.Elements.SharedStringTable) then
        { Counts = getCountsCore xmlReader
          StringItems = xmlReader |> getStringItemsCore |> ReadOnlyArray.ofSeq }
    else
        invalidOp $"{SharedStringNames.Elements.SharedStringTable} XML element not found in {SharedStringNames.Entry}"
