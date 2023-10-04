namespace FlattenedFSharpOptionsInSwagger.Mvc.AspNet

open System.Text.Json.Serialization
open System.Text.RegularExpressions

open Asp.Versioning
open Asp.Versioning.ApiExplorer

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Routing
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Mvc.ApplicationModels


type MvcJsonOptions = JsonOptions

type SnakeCaseTransformer() =
    interface IOutboundParameterTransformer with
        member this.TransformOutbound(value) =
            if isNull value then
                null
            else
                Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower()

[<RequireQualifiedAccess>]
module Api =

    let configureControllers (options: MvcOptions) =
        options.Conventions.Add(RouteTokenTransformerConvention(SnakeCaseTransformer()))

    let jsonFSharpOptions jsonSerializerOptions =
        JsonFSharpOptions
            .Default()
            .AddToJsonSerializerOptions(jsonSerializerOptions)

    let configureMvcJsonOption (options: MvcJsonOptions) =
        jsonFSharpOptions options.JsonSerializerOptions

    let configureRouting (options: RouteOptions) =
        options.ConstraintMap[ "snake-case" ] <- typeof<SnakeCaseTransformer>

    let setupVersioning (options: ApiVersioningOptions) =
        options.ApiVersionReader <- UrlSegmentApiVersionReader()
        options.ReportApiVersions <- true

    let setupVersionExplorer (options: ApiExplorerOptions) =
        options.GroupNameFormat <- "'v'VV"
        options.SubstituteApiVersionInUrl <- true

    let configureEndpoints (builder: IEndpointRouteBuilder) =
         builder.MapControllers()
         |> ignore
