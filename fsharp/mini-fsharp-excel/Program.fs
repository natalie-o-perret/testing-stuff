open System.IO
open System.IO.Compression

open FSharpPlus
open FSharpPlus.Data

open MiniExcelFSharp.Helpers
open MiniExcelFSharp.Xlsx.Entries.Constants


[<EntryPoint>]
let main _ =
    // ~12MB+ *.xlsx file
    //let path = @"C:\Users\natalie-perret\Desktop\accruals_extract20230410.xlsx"

    let path = @"C:\Users\natalie-perret\Desktop\XlsxShite\folders\stuff\stuff.xlsx"
    use fileStream = File.OpenRead(path)
    use zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read)
    let entry = zipArchive.GetEntry(SharedStringNames.Entry)
    use stream = entry.Open()
    use xmlReader = SimpleXmlReader.ofStream stream
    SimpleXmlReader.readElements xmlReader |> Seq.iter (fun x -> printfn $"%A{x}")

    (*
    let sharedStrings = SharedStrings.getAll zipArchive // 133324 values

    for KeyValue(sheetName, relPath) in WorkbookRels.getWorksheets zipArchive do
        printfn $"%s{sheetName}"
        let sw = Stopwatch.StartNew()

        Worksheet.getRows relPath zipArchive
        |> Seq.iter (fun row ->
            let cells =
                row.Cells
                |>> (fun cell ->
                    match cell.Type, cell.Value with
                    | "s", Some value -> value |> int32 |> ReadOnlyArray.tryAt sharedStrings
                    | _ -> cell.Value)

            printfn $"%A{row.No} - %A{cells}")
        // need to add proper benchmarking
        printfn $"%s{relPath} took %d{sw.ElapsedMilliseconds}ms"
    *)
    0
