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

proof pred_calculus <@ exists' x N ==> ((exists x N (P ||| Q)) = P ||| exists x N Q) @> [
    Dual |> L
    Dual |> L |> R'
]