namespace TypingSpeed

open Giraffe
open Api
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

type Startup() =
    member __.ConfigureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment) =
        app.UseDefaultFiles() |> ignore
        app.UseStaticFiles() |> ignore

        app.UseGiraffe (webApp  env.ContentRootPath)


