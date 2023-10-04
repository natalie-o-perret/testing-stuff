namespace FlattenedFSharpOptionsInSwagger.Mvc.Controllers

open System.Net.Mime
open Asp.Versioning

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

type GrandChild =
    { DoubleOptionSome: double option
      Int32OptionSome: int32 option
      StringOptionSome: string option
      DoubleOptionNone: double option
      Int32OptionNone: int32 option
      StringOptionNone: string option
      Double: double
      Int32: int32
      String: string }
type Child =
    { DoubleOptionSome: double option
      Int32OptionSome: int32 option
      StringOptionSome: string option
      DoubleOptionNone: double option
      Int32OptionNone: int32 option
      StringOptionNone: string option
      Double: double
      Int32: int32
      String: string
      Child: GrandChild option
      AnotherText: string option }
type Parent =
    { DoubleOptionSome: double option
      Int32OptionSome: int32 option
      StringOptionSome: string option
      DoubleOptionNone: double option
      Int32OptionNone: int32 option
      StringOptionNone: string option
      Double: double
      Int32: int32
      String: string
      Child: Child
      ChildOptionSome: Child option
      ChildOptionNone: Child option }

[<ApiController>]
[<ApiVersion("1.0")>]
[<Route("api/v{version:apiVersion}/[controller]")>]
type TmpController() =
    inherit ControllerBase()

    /// <summary>
    /// Post Shenanigans.
    /// </summary>
    [<HttpPost("shenanigans")>]
    [<Consumes(MediaTypeNames.Application.Json)>]
    [<Produces(MediaTypeNames.Text.Plain)>]
    [<ProducesResponseType(StatusCodes.Status200OK)>]
    [<ProducesResponseType(StatusCodes.Status400BadRequest)>]
    member this.PostShenanigans(parent: Parent) =
        this.Ok(sprintf $"%A{parent}")
