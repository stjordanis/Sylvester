﻿namespace Sylvester

open System
open System.Collections
open System.Collections.Generic
open System.Linq
open System.Numerics


/// A set of elements belonging to a universe denoted by U.
type Set<'U when 'U: equality> =
/// The empty set
| Empty

/// A single element of U.
| Elem of 'U

/// A sequence of elements i.e. a function from N -> U.
| Seq of seq<'U>

/// A set of elements of U defined by a predicate.
| Set of ('U -> bool)
    
with 
    /// Set union operator.
    static member inline (|+|) (l, r) = 
        match (l, r) with
        |(Empty, x) -> x
        |(x, Empty) -> x
        
        |(Elem _, _) -> failwith "Set union operation not defined for a set element."
        |(_, Elem _) -> failwith "Set union operation not defined for a set element."

        |(Seq a, Seq b) -> Seq.concat([a; b]) |> Seq
        |(Set a, Set b) -> Set(fun x -> a (x) || b(x))

        |(Set a, Seq b) -> Set(fun x -> a(x) || x |> Seq.contains b)
        |(Seq a, Set b) -> Set(fun x -> x |> Seq.contains a || b(x))

    /// Set intersection operator.
    static member inline (|*|) (l, r) = 
        match (l, r) with
        |(Empty, _) -> Empty
        |(_, Empty) -> Empty
        
        |(Elem _, _) -> failwith "Set intersection operation not defined for a set element."
        |(_, Elem _) -> failwith "Set intersection operation not defined for a set element."

        |(Seq a, Seq b) -> a.Intersect(b) |> Seq
        |(Set a, Set b) -> Set(fun x -> a (x) && b(x))

        |(Set a, Seq b) -> Set(fun x -> a(x) && x |> Seq.contains b)
        |(Seq a, Set b) -> Set(fun x -> x |> Seq.contains a && b(x))

    interface IEnumerable<'U> with
        member x.GetEnumerator () = 
            match x with
            |Empty -> Seq.empty.GetEnumerator()
            |Elem a -> Seq.singleton(a).GetEnumerator()
            |Seq s -> let distinct = Seq.distinct s in distinct.GetEnumerator()
            |Set s -> failwith "Cannot enumerate an arbitrary set. Use a sequence instead."
                
    interface IEnumerable with
        member x.GetEnumerator () = (x :> IEnumerable<'U>).GetEnumerator () :> IEnumerator

    /// A subset of the set.
    member x.Sub(f: 'U -> bool) = 
        match x with
        |Empty -> failwith "The empty set has no subsets."
        |Elem a -> failwith "A single element has no subsets."
        |Seq s -> Seq(s |> Seq.filter f)
        |Set s -> Set(fun x -> s(x) && f(x))

[<AutoOpen>]
module Set =
    let infiniteSeq f = f |> Seq.initInfinite |> Seq  

    let infiniteSeq2 f = f |> infiniteSeq |> Seq.pairwise