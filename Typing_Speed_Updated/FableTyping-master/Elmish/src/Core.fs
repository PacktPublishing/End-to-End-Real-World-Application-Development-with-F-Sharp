module Core

open Model
open System
open Elmish

let zeroTime = [0;0;0;0]
let updateTime (timer : Time) : Time =
    let t3 =  (timer.[3] + 1) |> float
    let t0 =  (t3/100./60.) |> Math.Floor
    let t1 =  (t3/100. - t0 * 60.) |> Math.Floor
    let t2 =  (t3 - t1 * 100.- t0 * 6000.) |> Math.Floor
    [int t0;int t1;int t2; int t3]

let update startTimer stopTimer message (model : TypingModel) =
    match message with
    | Tick -> { model with Time = updateTime model.Time}, Cmd.none
    | StartOver ->  {model with Status = Initial; Time = zeroTime; CurrentText = ""}, stopTimer
    | KeyPress when model.Status = Initial -> { model with Status = JustStarted}, startTimer

    | TextUpdated text when model.Status <> Complete ->
        let model = {model with CurrentText = text}

        if model.CurrentText = model.TargetText then
            {model with Status = Complete}, stopTimer
        else if (let originTextMatch = model.TargetText.Substring(0, model.CurrentText.Length)  
            model.CurrentText = originTextMatch) then
            { model with Status = Correct}, Cmd.none
        else
            {model with Status = Wrong}, Cmd.none

    | _ -> model, Cmd.none

