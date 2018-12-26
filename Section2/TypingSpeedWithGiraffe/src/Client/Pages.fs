module Pages
open Elmish.Browser.UrlParser

[<RequireQualifiedAccess>]
type Page =
    | Home
    | FastestTime

let toPath =
    function
    | Page.Home -> "/"
    | Page.FastestTime -> "/fastestTime"

/// The URL is turned into a Result.
let pageParser : Parser<Page -> Page,Page> =
    oneOf
        [ map Page.Home (s "")
          map Page.FastestTime (s "fastestTime") ]

let urlParser location = parsePath pageParser location


open Fable.Helpers.React.Props
open Fable.Core.JsInterop
open Fable.Import

open Elmish.Browser.Navigation

module R = Fable.Helpers.React
let goToUrl (e: React.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let viewLink page description =

  R.a [ Style [ Padding "0 20px" ]
        Href (toPath page)
        OnClick goToUrl]
      [ R.str description]