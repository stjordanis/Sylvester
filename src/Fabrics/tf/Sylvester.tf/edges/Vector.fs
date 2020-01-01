﻿namespace Sylvester.tf

open System
open System.Collections.Generic;
open System.Runtime.CompilerServices

open TensorFlow

open Sylvester
open Sylvester.Arithmetic
open Sylvester.Arithmetic.N10
open Sylvester.Collections
open Sylvester.Graphs
open Sylvester.Tensors

[<AutoOpen>]
module Vector =
    type Vector<'dim0, 't when 'dim0 :> Number and 't:> ValueType and 't : struct and 't: (new: unit -> 't) and 't :> IEquatable<'t> and 't :> IFormattable and 't :> IComparable>(graph:ITensorGraph, name:string, head:Node, output:int) =
        inherit Tensor<one, 't>(graph, name, head, output, [|number<'dim0>.Val|])
        interface IVector
        member x.Dim0:'dim0 = number<'dim0>

        new(name:string) = 
            let g = TensorGraph<zero, zero>.DefaultGraph
            let shape = [|number<'dim0>.Val|]
            new Vector<'dim0, 't>(g, name, new Node(g, "Placeholder", tf(g).Placeholder(dataType<'t>, shape), []), 0)

        static member (+)(l:Vector<'dim0, 't>, r:Vector<'dim0, 't>) = 
            let edgeName = l.TensorGraph.MakeName(sprintf "Add_%s_%s" l.Name r.Name) 
            let op = add l.Head r.Head
            let node = Node(l.TensorGraph, op.Name, op.Op.[0], [l :> Edge; r:>Edge])
            new Vector<'dim0, 't>(l.TensorGraph, edgeName, node, 0)
        
        static member (-)(l:Vector<'dim0, 't>, r:Vector<'dim0, 't>) = 
            let edgeName = l.TensorGraph.MakeName(sprintf "Sub_%s_%s" l.Name r.Name) 
            let op = sub l.Head r.Head
            let node = Node(l.TensorGraph, op.Name, op.Op.[0], [l :> Edge; r:>Edge])
            new Vector<'dim0, 't>(l.TensorGraph, edgeName, node, 0)
      
        static member (*)(l:Vector<'dim0, 't>, r:Vector<'dim0, 't>) = 
            let edgeName = l.TensorGraph.MakeName(sprintf "Mul_%s_%s" l.Name r.Name) 
            let op = mul l.Head r.Head
            let node = Node(l.TensorGraph, op.Name, op.Op.[0], [l :> Edge; r:>Edge])
            new Vector<'dim0, 't>(l.TensorGraph, edgeName, node, 0)

        static member (/)(l:Vector<'dim0, 't>, r:Vector<'dim0, 't>) = 
            let edgeName = l.TensorGraph.MakeName(sprintf "Div_%s_%s" l.Name r.Name) 
            let op = div l.Head r.Head
            let node = Node(l.TensorGraph, op.Name, op.Op.[0], [l :> Edge; r:>Edge])
            new Vector<'dim0, 't>(l.TensorGraph, edgeName, node, 0)