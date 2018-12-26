module Main
open Pages

module Model = 
    open Home.Model
    open Elmish
    open TimeUtil
    
    type PageModel =
        | HomePageModel of TypingModel
        | FastestTimeModel of Time

    type Message = 
        | HomeMessage of Home.Model.Message
        | FastestTimeMessage of Time

    let init (page : Page option) =
        match page with 
        | Some Page.FastestTime ->  
            let model, cmd  = FastestTime.init()
            FastestTimeModel model, Cmd.map FastestTimeMessage cmd
        | _ ->
            let model, cmd  = Home.init()
            HomePageModel model, Cmd.map HomeMessage cmd

module Core =
    open Elmish
    open Model
    let update (message : Message) (model : PageModel) = 
        match message, model with
        | HomeMessage msg, HomePageModel m -> 
            let model, cmd = Home.update msg m
            HomePageModel model, Cmd.map HomeMessage cmd
        | FastestTimeMessage msg, FastestTimeModel m -> 
            let model, cmd = FastestTime.update msg m
            FastestTimeModel model, Cmd.map FastestTimeMessage cmd
        | _ -> model, Cmd.none

module View =
    open Model
    let root (model : PageModel) (dispatch  : Message -> unit) = 
        match model with 
        | HomePageModel m ->
            Home.view m (HomeMessage >> dispatch)
        | FastestTimeModel t -> 
            FastestTime.view t (FastestTimeMessage >> dispatch)
    