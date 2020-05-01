module TypingModel

open Elmish
open Elmish.React
open Feliz

open Model
open Core
open Elmish
open Fable.Core.JsInterop
open Browser.Dom
open Browser.Types
open Fable
open Fable.Core
open Fable.React
open Fable.Core
open Fable.React
open Fable.React.Props
open Browser.Types
open Browser
open Elmish
open Elmish.React

let originText = document.querySelector("#origin-text p").innerHTML

let startTimer =
    let sub dispatch =
         if !!(window?myInterval) |> isNull then
            let interval = window.setInterval  ((fun () -> dispatch Tick), 10, [])
            window?myInterval <- interval
    Cmd.ofSub sub

let stopTimer =
    let sub _ =
        window.clearInterval !!(window?myInterval)
        window?myInterval <- null
    Cmd.ofSub sub

let viewTime (timer : Time) =
        timer.[0..2]
        |> List.map (fun s -> s.ToString("00")) 
        |> String.concat ":"

let error _ _ = div[][str "Rendering error"]

let getBorderColor = function
    | Initial  | JustStarted  -> "grey"
    | Correct -> "#65CCf3"
    | Wrong -> "#E95D0F"
    | Complete ->  "#429890"

let root model dispatch =
     div [][
        div [Class "test-wrapper"; Style [BorderColor <| !!getBorderColor model.Status] ] [
            textarea [
                Rows 6
                Value <| !!model.CurrentText
                Placeholder "The clock starts when you start typing"
                OnChange (fun e -> dispatch (TextUpdated !!e.target?value))
                OnKeyPress (fun e -> if e.target?value = "" then  dispatch KeyPress)
            ][]
        ]
        div [Class "meta"] [
            section [Id "clock"][
                div [Class "timer"][model.Time |> viewTime |> str ]
            ]
            button  [
                Id "reset"
                OnClick (fun _ -> dispatch StartOver) 
            ][str "Start over"]
        ]
    ]
let view model dispatch =
    root model dispatch

open Elmish.React

let init () =
    { Status = Initial; 
        CurrentText = ""; 
        TargetText = originText; 
        Time = zeroTime}, Cmd.none

let update' = update startTimer stopTimer

Program.mkProgram init update' view
  |> Program.withReactSynchronous "elmish-app"
  |> Program.run