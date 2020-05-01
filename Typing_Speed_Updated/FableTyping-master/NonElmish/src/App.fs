module App

open Fable.Core.JsInterop
open Fable.Import
open Browser.Types
open Browser.Dom
open Model
open Core

let testWrapper = document.querySelector(".test-wrapper")  :?> HTMLElement
let testArea = document.querySelector("#test-area") :?> HTMLTextAreaElement
let originText = document.querySelector("#origin-text p").innerHTML
let resetButton = document.querySelector("#reset") :?> HTMLButtonElement
let theTimer = document.querySelector(".timer") :?> HTMLElement

let model = 
    { Status = Initial; 
        CurrentText = ""; 
        TargetText = originText; 
        Time =[0;0;0;0] } : TypingModel
let viewTime (timer : Time) =
    let leadingZero section =
        if (section <= 9) then
            "0" + section.ToString()
        else
            section.ToString()
    let currentTime = leadingZero(timer.[0]) + ":" + leadingZero(timer.[1]) + ":" + leadingZero(timer.[2]);
    theTimer.innerHTML <- currentTime;

let stopTimer () =
    window.clearInterval !!(window?myInterval)
    window?myInterval <- null

let view {Status = status ; Time = time} (dispatcher: MailboxProcessor<Message>) =
    match status with
    | Initial ->
        testArea.value <- ""
        theTimer.innerHTML <- "00:00:00"
       // testWrapper.style.borderColor <- "grey"
        stopTimer()
    | JustStarted ->
        if !!(window?myInterval) |> isNull then
            let interval = window.setInterval  ((fun () -> dispatcher.Post Tick), 10, [])
            window?myInterval <- interval
    | Correct ->
        testWrapper?style?borderColor <- "#65CCf3"
        //()
        //testWrapper.

    | Wrong ->
        testWrapper?style?borderColor <- "#E95D0F"
       //()

    | Complete ->
        testWrapper?style?borderColor <- "#429890"
        stopTimer()

    viewTime time



#nowarn "40"
let rec dispatcher = MailboxProcessor<Message>.Start(fun inbox->

    // the message processing function
    let rec messageLoop (model : TypingModel) = async{
        // read a message
        let! msg = inbox.Receive()
        // process a message
        let newModel = update model msg
        view newModel dispatcher
        // loop to top
        return! messageLoop newModel}

    // start the loop
    messageLoop model)

testArea.addEventListener("keyup", fun e -> dispatcher.Post (TextUpdated !!(e.target?value)) |> ignore)
testArea.addEventListener("keypress", fun _ -> dispatcher.Post (KeyPress) |> ignore)
resetButton.addEventListener("click", fun _ -> dispatcher.Post (StartOver) |> ignore)

