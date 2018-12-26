module Home
open TimeUtil
open Fable.PowerPack
open Pages

module  Model =
    type Status =
        | Initial
        | JustStarted
        | Complete
        | Wrong
        | Correct

    type Message =
        | Tick
        | StartOver
        | KeyPress
        | TextUpdated of string
        | NOP

    type TypingModel = { Time : TimeUtil.Time; Status : Status; CurrentText : string; TargetText : string }



module private Core =

    open Elmish
    open Model
  

    let update startTimer stopTimer sendFastest message (model : TypingModel) =
        match message with
        | Tick -> { model with Time = updateTime model.Time}, Cmd.none
        | StartOver ->  {model with Status = Initial; Time = zeroTime; CurrentText = ""} , stopTimer
        | KeyPress when model.Status = Initial -> { model with Status = JustStarted} , startTimer

        | TextUpdated text when model.Status <> Complete ->
            let model = {model with CurrentText = text}

            if model.CurrentText = model.TargetText then
                {model with Status = Complete} , Cmd.batch[stopTimer; (sendFastest model.Time)]
            else if (let originTextMatch = model.TargetText.Substring(0, model.CurrentText.Length)
                model.CurrentText = originTextMatch) then
                { model with Status = Correct}, Cmd.none
            else
                {model with Status = Wrong}, Cmd.none

        | _ -> model, Cmd.none

module private View =
    open Elmish
    open Model
    open Fable.Core.JsInterop
    open Fable.Import.Browser
    open Fable.Helpers.React
    open Fable.Helpers.React.Props

    let originText = document.querySelector("#origin-text p").innerHTML

    let startTimer =
        let sub dispatch =
             if !!(window?myInterval) |> isNull then
                let interval = window.setInterval  ((fun () -> dispatch Tick), 10, [])
                window?myInterval <- interval
        Cmd.ofSub sub

    let stopTimer : Cmd<Message>=
        let sub _ =
            window.clearInterval !!(window?myInterval)
            window?myInterval <- null
        Cmd.ofSub sub

    let sendFastest time : Cmd<Message>=
        let p (fastestTime) =
            promise{
                let! r = Fetch.postRecord("/api/fastestTime") fastestTime []
                return r.Ok
            }
        Cmd.ofPromise p time (fun _ -> NOP) (fun _ -> NOP)

    let private viewTime (timer : Time) =
        timer.[0..2]
            |> Array.map (fun s -> s.ToString("00"))
            |> String.concat ":"

    let private error _ _ = div[][str "Rendering error"]

    let private getBorderColor = function
        | Initial  | JustStarted  -> "grey"
        | Correct -> "#65CCf3"
        | Wrong -> "#E95D0F"
        | Complete ->  "#429890"

    let root model dispatch =
         div [][
            viewLink Page.FastestTime "See fastest time"
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

open Model
open Elmish
let init () =
    {   Status = Initial;
        CurrentText = "";
        TargetText = View.originText;
        Time = zeroTime } , Cmd.none


let view = View.root

let update =  Core.update View.startTimer View.stopTimer View.sendFastest