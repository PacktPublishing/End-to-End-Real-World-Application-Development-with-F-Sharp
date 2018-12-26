module FastestTime
open Fable.PowerPack

    module Model =
        type Time = int array
        let zeroTime = [|0;0;0;0|]

    module private Core =
        open Model
        open Elmish
        let update  (message:Time) (model : Time) = 
            message, Cmd.none

    
    module private View =
        open Elmish
        open Model
        open Fable.Core.JsInterop
        open Fable.Import.Browser
        open Fable.Helpers.React
        open Fable.Helpers.React.Props
        open Fable.Import.React
        open Fable.Core
        type AnimaKitProps = Expanded of bool

        let inline animakit_expander (props : AnimaKitProps list) (elems : ReactElement list) : ReactElement =
            ofImport "default" "animakit-expander" (keyValueList CaseRules.LowerFirst props) elems
        let loadCmd = 
            let p () =
                promise {
                   let! r = Fetch.fetch("/api/fastestTime") []
                   return! r.json<Time>()
                }
            Cmd.ofPromise p () (fun r-> r) (fun _ -> zeroTime)
        let private viewTime (timer : Time) = 
            timer.[0..2]
                |> Array.map (fun s -> s.ToString("00"))
                |> String.concat ":"

        let root (model : Time) _  = 
            div [][
                Pages.viewLink Pages.Page.Home "Compete"
                animakit_expander [Expanded  true][
                    h1 [][str <| "Fastest Time is " + (model |> viewTime )]
                ]
            ]
          

    let init () =
       Model.zeroTime, (View.loadCmd)

    let view = View.root

    let update =  Core.update

