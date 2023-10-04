open System

open Vp.FSharp.Sql.Postgres
open Vp.FSharp.Sql.Sqlite


sqliteCommand {
    text ""
    noLogger
    parameters [ "hello", SqliteDbValue.Null ]
}
|> printfn "%A"

SqliteCommand.text ""
|> SqliteCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"

postgresCommand {
    text ""
    noLogger
    parameters [ "hello", PostgresDbValue.Null ]
}
|> printfn "%A"

PostgresCommand.text ""
|> PostgresCommand.timeout (TimeSpan.FromDays 2)
|> printfn "%A"
