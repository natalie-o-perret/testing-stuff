[<RequireQualifiedAccess>]
module MiniExcelFSharp.Xlsx.ZipArchive

open System.IO.Compression


let OpenEntry (source: ZipArchive) entryName =
    let entry = source.GetEntry(entryName)
    entry.Open()
