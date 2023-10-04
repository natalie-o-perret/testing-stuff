[<RequireQualifiedAccess>]
module MiniExcelFSharp.Helpers.SimpleXmlReader

open System.IO
open System.Xml
open System.Diagnostics

open FSharp.Control

open MiniExcelFSharp.Xlsx.ReadOnlyWrapper


let ofStream (stream: Stream) =
    let settings = XmlReaderSettings()
    settings.IgnoreComments <- true
    settings.IgnoreWhitespace <- true
    settings.XmlResolver <- null
    XmlReader.Create(stream, settings)


let inline private tryGetCurrentParentDepth (source: XmlReader) =
    // Save the element or root depth
    let mutable parentDepth = source.Depth

    if source.NodeType <> XmlNodeType.Element then
        // Adjust the depth if we are on root node
        if source.ReadState = ReadState.Initial then
            Debug.Assert((parentDepth = 0))
            Some(parentDepth - 1)
        else
            None
    elif source.IsEmptyElement then
        None
    else
        Some(parentDepth)

let readToFirstDescendantWithAnyOf (source: XmlReader) localNames =

    if Set.count localNames = 0 then
        $"{nameof localNames} must not be null or empty."
        |> invalidArg (nameof localNames)
    else
        // Save the element or root depth
        match tryGetCurrentParentDepth source with
        | Some parentDepth ->
            let mutable outcome = None

            while outcome.IsNone && source.Read() && (source.Depth > parentDepth) do
                if
                    source.NodeType = XmlNodeType.Element
                    && Set.contains source.LocalName localNames
                then
                    outcome <- Some(source.LocalName)

            if outcome.IsSome then
                outcome
            else
                Debug.Assert(
                    source.NodeType = XmlNodeType.EndElement
                    || source.NodeType = XmlNodeType.None
                    || source.ReadState = ReadState.Error
                )

                None
        | None -> None

let inline getAttributeAt (source: XmlReader) index =
    source.MoveToAttribute(i = index)
    source.Name, source.Value

let inline getAttributes (source: XmlReader) =
    Seq.init source.AttributeCount (getAttributeAt source) |> ReadOnlyDict.ofTuples

let inline private readElement (source: XmlReader) =
    if source.Read() && source.NodeType = XmlNodeType.Element then
        Some(source.Depth, source.Name, getAttributes source, source.Value)
    else
        None

let inline private readElementTask (source: XmlReader) =
    task {
        let! isThereAnything = source.ReadAsync()

        if isThereAnything && source.NodeType = XmlNodeType.Element then
            return Some(source.Depth, source.Name, getAttributes source, source.Value)
        else
            return None
    }

let getCurrentXmlElement (source: XmlReader) =
    { Name = source.Name
      Depth = source.Depth
      Attributes = getAttributes source }

let readElements (source: XmlReader) =
    seq {
        let mutable maybePrevElement = None

        while source.Read() do
            match source.NodeType, maybePrevElement with
            | XmlNodeType.Element, Some prevElement ->
                yield prevElement, None
                maybePrevElement <- source |> getCurrentXmlElement |> Some
            | XmlNodeType.Element, None -> maybePrevElement <- source |> getCurrentXmlElement |> Some
            | XmlNodeType.Text, Some prevElement ->
                yield prevElement, Some source.Value
                maybePrevElement <- None
            | _ -> ()

        match maybePrevElement with
        | Some prevElement -> yield prevElement, None
        | None -> ()
    }

let inline readElementsTaskSeq (source: XmlReader) =
    taskSeq {
        let mutable maybePrevElement = None
        let! keepReadingInit = source.ReadAsync()
        let mutable keepReading = keepReadingInit

        while keepReading do
            match source.NodeType, maybePrevElement with
            | XmlNodeType.Element, Some prevElement ->
                yield prevElement, None
                maybePrevElement <- source |> getCurrentXmlElement |> Some
            | XmlNodeType.Element, None -> maybePrevElement <- source |> getCurrentXmlElement |> Some
            | XmlNodeType.Text, Some prevElement ->
                yield prevElement, Some source.Value
                maybePrevElement <- None
            | _ -> ()

            let! keepReadingNext = source.ReadAsync()
            keepReading <- keepReadingNext

        match maybePrevElement with
        | Some prevElement -> yield prevElement, None
        | None -> ()
    }

let inline readElementsAsyncSeq source =
    readElementsTaskSeq source |> AsyncSeq.ofAsyncEnum
