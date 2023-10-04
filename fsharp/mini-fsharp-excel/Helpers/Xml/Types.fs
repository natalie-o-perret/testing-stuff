namespace MiniExcelFSharp.Helpers

open MiniExcelFSharp.Xlsx.ReadOnlyWrapper


type SimpleXmlElement =
    { Name: string
      Depth: int32
      Attributes: ReadOnlyDict<string, string> }
