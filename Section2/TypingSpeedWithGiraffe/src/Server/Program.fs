namespace TypingSpeed

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting

module Program =
    let CreateWebHostBuilder args =
        WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>();

    [<EntryPoint>]
    let main args =
        CreateWebHostBuilder(args).Build().Run()
        0
