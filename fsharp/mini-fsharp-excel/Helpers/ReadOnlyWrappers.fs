namespace MiniExcelFSharp.Xlsx.ReadOnlyWrapper

open System.ComponentModel
open System.Collections.Generic


[<Struct>]
type ReadOnlyArray<'T> (source: 'T array) =
    [<Browsable(false)>]
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    member _.``🚫`` = source
    member inline this.Item with get index = this.``🚫``[index]
    member inline this.Length = this.``🚫``.Length
    override this.ToString() = sprintf $"%A{this.``🚫``}"

[<RequireQualifiedAccess>]
module ReadOnlyArray =
    let inline at (source: ReadOnlyArray<'T>) (index: int32) = source.[index]
    let inline tryAt (source: ReadOnlyArray<'T>) (index: int32) =
        if index < 0 || index >= source.Length then None
        else Some source.[index]
    let inline map mapping (source: ReadOnlyArray<'T>) =
        ReadOnlyArray [| for i in 0 .. source.Length - 1 do yield mapping source.[i] |]
    let inline tryFind predicate (source: ReadOnlyArray<'T>) =
        let rec loop i =
            if i >= source.Length then None
            elif predicate source.[i] then Some source.[i]
            else loop (i + 1)
        loop 0

    let inline ofArray (source: 'T array) = ReadOnlyArray(source)
    let inline ofSeq (source: seq<'T>) = source |> Seq.toArray |> ReadOnlyArray

[<Struct>]
type ReadOnlyDict<'Key, 'Value> (source: Dictionary<'Key, 'Value>) =

    [<Browsable(false)>]
    [<EditorBrowsable(EditorBrowsableState.Never)>]
    member _.``🚫`` = source
    member inline this.Item with get key = this.``🚫``[key]

    interface IEnumerable<KeyValuePair<'Key, 'Value>> with
        member this.GetEnumerator(): IEnumerator<KeyValuePair<'Key,'Value>> = this.``🚫``.GetEnumerator()
        member this.GetEnumerator(): System.Collections.IEnumerator = this.``🚫``.GetEnumerator()
    override this.ToString() = sprintf $"%A{this.``🚫``}"


[<RequireQualifiedAccess>]
module ReadOnlyDict =
    let inline ofTuples (source: ('Key * 'Value) seq) =
        let d = Dictionary<'Key, 'Value>()
        for key, value in source do d.Add(key, value)
        ReadOnlyDict(d)

    let inline filter ([<InlineIfLambda>] predicate) (source: ReadOnlyDict<'Key, 'Value>) =
        let d = Dictionary<'Key, 'Value>()
        for KeyValue(key, value) in source.``🚫`` do if predicate key value then d.Add(key, value)
        ReadOnlyDict(d)
