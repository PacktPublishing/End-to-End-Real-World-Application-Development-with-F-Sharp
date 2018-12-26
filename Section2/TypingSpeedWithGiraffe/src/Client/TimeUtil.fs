module TimeUtil
open System
type Time = int array
let zeroTime = [|0;0;0;0|]

let updateTime (timer : Time) : Time =
    let t3 =  (timer.[3] + 1) |> float
    let t0 =  (t3/100./60.) |> Math.Floor
    let t1 =  (t3/100. - t0 * 60.) |> Math.Floor
    let t2 =  (t3 - t1 * 100.- t0 * 6000.) |> Math.Floor
    [|int t0;int t1;int t2; int t3|]