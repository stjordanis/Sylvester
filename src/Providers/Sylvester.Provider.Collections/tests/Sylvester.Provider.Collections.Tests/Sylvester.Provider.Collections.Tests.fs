module Sylvester.Provider.CollectionsTests

open Sylvester.Arithmetic
open Sylvester.Arithmetic.N10
open Sylvester.Collections
open NUnit.Framework

type htype = Array<100>
[<Test>]
let ``Default constructor should create instance`` () =    
    let v = Array<100>.create([|43|])
    
    let s (r: VArray<_1,_0,_0>) = ()

    s v
    
    ()


    //Assert.AreEqual("My internal state", MyType().InnerState)

 