﻿namespace Sylvester

open System
open System.Collections

open FSharp.Quotations
open FSharp.Quotations.Patterns
open Sylvester.Arithmetic
open Sylvester.Collections

type IReflexiveRelation = interface end

type ISymmetricRelation = interface end

type IAntiSymmetricRelation = interface end

type ITransitiveRelation = interface end

type IEquivalenceRelation =
    inherit IReflexiveRelation
    inherit ISymmetricRelation
    inherit ITransitiveRelation

type Relation<'t when 't : equality>(set: Set<'t>, [<ReflectedDefinition(true)>] test:Expr<('t * 't) -> bool>) =
    let v = match test with | WithValue(v, _, _) -> v | _ -> failwith "Unexpected expression."
    member val ParentSet = set
    member val Pred = v :?> ('t *'t) -> bool
    member val Expr = test
    member x.Set = set.Product.Subset(x.Pred)
    interface ISet<'t * 't> with 
        member x.Set = x.Set
        member x.Equals y = x.Set.Equals y
    interface Generic.IEnumerable<'t * 't> with
        member x.GetEnumerator(): Generic.IEnumerator<'t * 't> =
            let s = x.Set :> Generic.IEnumerable<'t *'t> in s.GetEnumerator()
        member x.GetEnumerator(): IEnumerator = (x.Set :> Generic.IEnumerable<'t * 't>).GetEnumerator () :> IEnumerator
    
//[<AutoOpen>]
//module Relation = 
 //   type Set<'t when 't : equality> with
 //       member x.Relation(f: ('t * 't) -> bool) = Relation(x, f)
