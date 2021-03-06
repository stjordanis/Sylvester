//#r ".\\..\\..\\src\\Providers\\Sylvester.Provider.Arithmetic\\src\\Sylvester.Provider.Arithmetic.Runtime\\bin\\Release\\netstandard2.0\\Sylvester.Provider.Arithmetic.Runtime.dll"
//#r ".\\..\\..\\src\\Base\\Sylvester.Collections\\bin\\Debug\\netstandard2.0\\Sylvester.Collections.dll"
//#r ".\\..\\..\\src\\Lang\\Sylvester.Expressions\\bin\\Debug\\netstandard2.0\\Sylvester.Expressions.dll"

#r ".\\..\\..\\ext\\FunScript\\src\\main\\FunScript\\bin\\Debug\\netstandard2.0\\FunScript.Util.dll"
#r ".\\..\\..\\ext\\FunScript\\src\\main\\FunScript\\bin\\Debug\\netstandard2.0\\FunScript.Interop.NETStandard.dll"
#r ".\\..\\..\\ext\\FunScript\\src\\main\\FunScript\\bin\\Debug\\netstandard2.0\\FunScript.dll"
#r ".\\..\\..\\ext\\FunScript\\src\\main\\FunScript.Bindings.Base\\bin\\Debug\\netstandard2.0\\FunScript.Bindings.Base.dll"
#r ".\\..\\..\\ext\\FunScript\\src\\extra\\FunScript.D3\\bin\\Debug\\netstandard2.0\\FunScript.D3.dll"


open System
open FunScript
open FunScript.Compiler
open FunScript.Bindings

[<ReflectedDefinition>]
[<AutoOpen>]
module Foo =
    // Create a function that will be compiled into JavaScript...
    let htmlCanvasTS()=
        let canvas = Globals.document.getElementsByTagName_canvas().[0]
        canvas.width <- 1000.
        canvas.height <- 800.
        let ctx = canvas.getContext_2d()
        ctx.fillStyle <- "rgb(200,0,0)"
        ctx.fillRect (10., 10., 55., 50.);
        ctx.fillStyle <- "rgba(0, 0, 200, 0.5)"
        ctx.fillRect (30., 30., 55., 50.)

let js =  Compiler.Compile(<@ htmlCanvasTS @>, noReturn=true)
printf "%s" js
//<@ htmlCanvasTS()@>
