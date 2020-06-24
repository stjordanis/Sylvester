#load "Include.fsx"

open Sylvester
open PropCalculus
open PredCalculus


let p,q,r,s = var4<bool>
let x,y = var2<bool> 
let p',q',r',s' = <@ p @>, <@ q @>, <@ r @>, <@ s @>
let x' = <@ x @>
let y' = <@ y @>
let i, n, m = var3<int>
let P,Q, N, A= var4<bool>
let P',Q', N' = <@ P @>, <@ Q @>, <@ N @>

//let c = new Wit
let M = pred<int>

let W = pred<int>
proof pred_calculus <@ forall' i (M i ==> W i) ==> false @> [

]
