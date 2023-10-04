[<RequireQualifiedAccess>]
module FlattenedFSharpOptionsInSwagger.Mvc.AspNet.Startup

open System.Reflection

open Microsoft.FSharp.Core
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection


let buildAndRun (args: string array) =

    let builder = WebApplication.CreateBuilder(args)

    builder.Services
        .AddControllers(Api.configureControllers)
        .AddJsonOptions(Api.configureMvcJsonOption)
        .AddApplicationPart(Assembly.GetEntryAssembly())
        .Services
        .AddApiVersioning(Api.setupVersioning)
        .AddApiExplorer(Api.setupVersionExplorer)
        .Services
        .AddRequestDecompression()
        .AddResponseCompression()
        .AddRouting(Api.configureRouting)
        .AddSwaggerGen(Swagger.setupGen builder.Services)
        |> ignore

    let app = builder.Build()

    app
        .UseDeveloperExceptionPage()
        .UseStaticFiles()
        .UseSwagger()
        .UseSwaggerUI(Swagger.setupUi (app.DescribeApiVersions()))
        .UseRequestDecompression()
        .UseResponseCompression()
        .UseHttpsRedirection()
        .UseRouting()
        .UseEndpoints(Api.configureEndpoints)
    |> ignore

    app.Run()
