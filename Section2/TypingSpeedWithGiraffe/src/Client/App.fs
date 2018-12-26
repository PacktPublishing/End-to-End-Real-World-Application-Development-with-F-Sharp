module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open Elmish.Debug
open Elmish.HMR

open Pages
open Elmish.Browser
open Elmish.ReactNative
open Main.Model

let urlUpdate (result:Page option) (model : PageModel)  : PageModel * Cmd<Message>=
    printf "%A" result
    System.Console.WriteLine("dfads")
    match result with
    | Some Page.FastestTime ->
        let m, cmd = FastestTime.init()
        FastestTimeModel m, Cmd.map FastestTimeMessage cmd
    | _ ->     
        let m, cmd = Home.init()
        HomePageModel m, Cmd.map HomeMessage cmd

Program.mkProgram init Main.Core.update Main.View.root
|> Program.toNavigable Pages.urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
//|> Program.withHMR
#endif
|> Program.withReactUnoptimized "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
