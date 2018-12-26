module Api

open System
open Akkling
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe

type Command =
    | Get
    | Set of int array

let system = System.create "typing-speed" <| Configuration.defaultConfig()

let fastestActor = spawnAnonymous system <| props(fun ctx ->
    let rec loop (state:int array) = actor {
        match! ctx.Receive() with
        | Set l when l.[3] < state.[3] -> return! loop l
        | Get ->
            ctx.Sender()  <! state
            return! loop state
        | _ -> return! loop state
    }

    loop [|10;0;0;100000|])

let getFastest =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! res = fastestActor <? Get
            return! json res next ctx
        }
let setFastest =
    fun _ (ctx : HttpContext) ->
        task{
            let! state = ctx.BindJsonAsync<int array>()
            fastestActor <! Set state
            return Some ctx}

let api : HttpHandler =
      subRoute "/api" <|
        choose [
            GET  >=> route "/fastestTime" >=> getFastest
            POST >=> route "/fastestTime" >=> setFastest
        ]
// HttpHandler : HttpFunc -> HttpContext -> Task<HttpContext option>
// HttpFunc : HttpContext -> Task<HttpContext option>
// 1: If the last of pipeline and we would to return a result: We return Some HttpContext
// 2: We don't want to process it but want to pass it to sibling HttpHandler: We return None
// 3: If we want to inspect the request and response but don't want to alter it: We invoke next
// 4: We still  return a result but also can invoke another http handler: We return Some Come HttpContext

let webApp rootPath : HttpHandler =
    choose [
        api
        GET >=> htmlFile (rootPath + "/wwwroot/index.html")
    ]
