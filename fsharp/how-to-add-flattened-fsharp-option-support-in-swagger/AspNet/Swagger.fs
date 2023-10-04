namespace FlattenedFSharpOptionsInSwagger.Mvc.AspNet

open System
open System.Text.Json

open FSharpPlus

open Microsoft.FSharp.Core

open TypeShape.Core.Core

open Asp.Versioning.ApiExplorer

open Microsoft.OpenApi.Models
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

open Swashbuckle.AspNetCore.SwaggerUI
open Swashbuckle.AspNetCore.SwaggerGen


// Replaces all Option<'T> types and types with Properties that have Option<'T> with the corresponding 'T type
type FSharpOptionSchemaFilter() =

    interface ISchemaFilter with
        member x.Apply(schema: OpenApiSchema, context: SchemaFilterContext) =
            match TypeShape.Create(context.Type) with
            | Shape.FSharpOption valueTypeShape ->
                let elementType = valueTypeShape.Element.Type
                let argumentSchema = context.SchemaGenerator.GenerateSchema(elementType, context.SchemaRepository)
                printfn $"%A{argumentSchema.Reference}: %A{elementType}"
                schema.Reference <- argumentSchema.Reference
                schema.Nullable <- true
            | _ ->
                for propertyInfo in context.Type.GetProperties() do
                    let camelCasePropertyName = JsonNamingPolicy.CamelCase.ConvertName(propertyInfo.Name)
                    match TypeShape.Create(propertyInfo.PropertyType) with
                    | Shape.FSharpOption valueTypeShape ->
                        let argumentType = valueTypeShape.Element.Type
                        let argumentSchema =
                            context.SchemaGenerator.GenerateSchema(argumentType, context.SchemaRepository)

                        schema.Properties[camelCasePropertyName].Reference <- argumentSchema.Reference
                        schema.Properties[camelCasePropertyName].Type <- argumentSchema.Type
                        schema.Properties[camelCasePropertyName].Nullable <- true
                        printfn $"%s{propertyInfo.Name}: %A{argumentType}"
                    | _ -> ()


type FsharpOptionDocumentFilter() =
    let [<Literal>] FSharpOptionSuffix = "FSharpOption"

    interface IDocumentFilter with
        member this.Apply(swaggerDoc, _) =
            swaggerDoc.Components.Schemas.Keys
            |> Seq.where (String.endsWith FSharpOptionSuffix)
            |> Seq.iter (swaggerDoc.Components.Schemas.Remove >> ignore)

[<RequireQualifiedAccess>]
module Swagger =

    let setupGen (services: IServiceCollection) (options: SwaggerGenOptions) =

        options.SchemaFilter<FSharpOptionSchemaFilter>()
        options.DocumentFilter<FsharpOptionDocumentFilter>()

        use serviceProvider = services.BuildServiceProvider()
        let apiVersionDescriptionProvider = serviceProvider.GetService<IApiVersionDescriptionProvider>()

        for description in apiVersionDescriptionProvider.ApiVersionDescriptions do
            let openApiContact = OpenApiContact()
            openApiContact.Name <- "Natoo"
            openApiContact.Url <- Uri("https://natoo-at-goofy.land")
            openApiContact.Email <- "natoo[at]goofy.land"
            let openApiInfo = OpenApiInfo()
            openApiInfo.Title <- "My A-MA-ZING API"
            openApiInfo.Description <- "A FUN-TAS-TIC-BU-LOU[i]S API"
            openApiInfo.Version <- $"{description.ApiVersion}"
            openApiInfo.Contact <- openApiContact
            options.SwaggerDoc(description.GroupName, openApiInfo)

    let setupUi  (apiVersionDescriptions: ApiVersionDescription seq) (options: SwaggerUIOptions) =
        options.InjectStylesheet("/swagger-ui/swagger-dark.css")

        let formatJsonSpecUrl = sprintf "/swagger/%s/swagger.json"

        for apiVersionDescription in apiVersionDescriptions do
            let jsonUrl = formatJsonSpecUrl apiVersionDescription.GroupName
            let name = apiVersionDescription.GroupName |> String.toUpper
            options.SwaggerEndpoint(jsonUrl, name)
