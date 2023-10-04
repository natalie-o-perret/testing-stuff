namespace MiniExcelFSharp.Xlsx.Entries

open System


type Column = string
type RowNumber = int32
type CellCoordinates = Column * RowNumber

type Count = int32
type UniqueCount = int32

type NumericalFormat = { Id: string; Code: string }

type Font =
    { Size: double
      Name: string
      Family: string }

type Fill = { PatternType: string }


type SharedStringType =
    | InlineString of Value: string
    | SharedString of Index: int32

type CellValueType =
    // t="b"
    | Boolean of Value: bool
    // t="d"
    | DateTime of Value: DateTime
    // t="e"
    | Error of Value: string
    // t="inlineStr" + <is><t>...</t></is>
    | String of Value: string
    // t="s" + <v>...</v>
    | SharedString of Index: int32
    // t="str" + <f><v></v></f>
    | Formula of Value: string

type WorksheetCell =
    { Column: string
      Type: string option
      Value: string option
      Style: int32 option
      Formula: string option }

type WorksheetRow =
    { RowNumber: int32
      Cells: WorksheetCell list }
