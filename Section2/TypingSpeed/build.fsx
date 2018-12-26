#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.JavaScript
open Fake.IO.FileSystemOperators
[<Literal>]
let FABLE_APP_DIR  = "src"

[<Literal>]
let WEBPACK_CONFIG_PATH = FABLE_APP_DIR + "/webpack.config.js"

let runFable args =
    let result =
        DotNet.exec
            (DotNet.Options.withWorkingDirectory (__SOURCE_DIRECTORY__ </> FABLE_APP_DIR))
            "fable" args
    if not result.OK then
        failwithf "dotnet fable failed with code %i" result.ExitCode


Target.create "YarnInstall" (fun _ ->
    Yarn.install id
)

Target.create "Build" (fun _ ->
    runFable <| "webpack-cli -- --config " + WEBPACK_CONFIG_PATH
)

Target.create "Watch" (fun _ ->
    runFable  <| "webpack-dev-server -- --config " + WEBPACK_CONFIG_PATH
)
Target.create "DotnetRestore" (fun _ ->
    DotNet.restore
        (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__)
        "TypingSpeed.sln"
)
Target.create "BuildDotNet" (fun _ ->
    DotNet.build
      (fun e-> 
        (DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__ 
            { e with Configuration =  DotNet.BuildConfiguration.Release }))
        "TypingSpeed.sln"
)

"DotnetRestore"
  ==> "YarnInstall"
  ==> "Build"

"YarnInstall"
    ==> "Watch"


Target.runOrDefault "Watch"
