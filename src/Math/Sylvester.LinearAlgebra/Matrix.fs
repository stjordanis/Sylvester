﻿namespace Sylvester

open System

open FSharp.Quotations

open Sylvester.Arithmetic

open MathNet.Numerics

//[<StructuredFormatDisplay("{Display}")>]
type Matrix<'t when 't:> ValueType and 't : struct and 't: (new: unit -> 't) and 't :> IEquatable<'t> and 't :> IFormattable and 't :> IComparable>
    ([<ParamArray>] data: 't array array, ?expr: Expr<'t list list>) =
    do if data |> Array.forall (fun a -> a.Length = data.[0].Length) |> not then failwith "The length of each column in a matrix must be the same."
    let matrix = lazy LinearAlgebra.DenseMatrix.ofColumnArrays data
    member val _Array = data
    member val _Matrix = matrix
    member val Expr = expr
    member val Display = sprintf "%A" matrix
    interface IPartialShape<two> with
        member val Rank = Some 2 with get,set
        member val Dims = [| data.[0].LongLength; data.LongLength |] |> Some with get,set
        member val Data = data :> Array with get, set
    new(x: Expr<'t list list>) = 
        let values = x |> expand_list_values<'t> |> List.map List.toArray |> List.toArray
        Matrix<'t>(values, x)

    static member Ops = defaultLinearAlgebraOps
    static member op_Explicit (x:Matrix<'t>) = x._Array
    static member op_Explicit (x:Matrix<'t>) = x._Matrix
    static member create(x: Array) = Matrix(x :?> 't [] [])
    static member create(x:_Matrix<'t>) = Matrix<'t>(let a = x.AsColumnArrays() in if not(isNull (a)) then a else x.ToColumnArrays()) 
    static member ToFloat(m:Matrix<'t>) = m._Array |> Array.map(fun ar -> ar |> Array.map (fun a -> Convert.ToDouble a)) |> Matrix
    static member ToInt(m:Matrix<'t>) = m._Array |> Array.map(fun ar -> ar |> Array.map (fun a -> Convert.ToInt32 a)) |> Matrix.create
    static member ToIntL(m:Matrix<'t>) = m._Array |> Array.map(fun ar -> ar |> Array.map (fun a -> Convert.ToInt64 a)) |> Matrix
    static member ToRational(m:Matrix<float>) = m._Array |> Array.map(fun ar -> ar |> Array.map (fun a -> Rational a)) |> Matrix
    static member (+)(l : Matrix<'t>, r : Matrix<'t>) = 
        match typeof<'t> with
        | MathNetLinearAlgebraSupportedType _ -> let res = Matrix<'t>.Ops.MatAdd l._Matrix.Value r._Matrix.Value in res |> Matrix.create  
        | t when t.Name = "Int32" -> let res = (+) (l |> Matrix<'t>.ToFloat) (r |> Matrix<'t>.ToFloat) in res |> Matrix.ToInt  
        | _ -> failwith "not supported yet"
        
    //static member (-)(l : Matrix<'t>, r : Matrix<'t>) = Matrix<'t>.Ops.MatSubtract l._Matrix r._Matrix
    //static member (*)(l : Matrix<'t>, r : Matrix<'t>) = Matrix<'t>.Ops.MatMultiply l._Matrix r._Matrix 

[<StructuredFormatDisplay("{Display}")>]
type Matrix<'dim0, 'dim1, 't when 'dim0 :> Number and 'dim1 :> Number and 't:> ValueType and 't : struct and 't: (new: unit -> 't) and 't :> IEquatable<'t> and 't :> IComparable and 't :> IFormattable>
    ([<ParamArray>] data: 't array array, ?expr: Expr<'t list list>) =
    inherit Matrix<'t>(data, ?expr = expr) 
    let d0, d1 = number<'dim0>.IntVal, number<'dim1>.IntVal
    do if base._Array.Length <> d0  || (base._Array |> Array.forall (fun a -> a.Length = d1) |> not) then failwithf "The initializing array does not have the dimensions %ix%i."d0 d1
    member val Dim0:'dim0 = number<'dim0>
    member val Dim1:'dim1 = number<'dim1>
    member val Display = ""
        //sprintf "Matrix<%i,%i>\n%s" base._Matrix.RowCount base._Matrix.ColumnCount (base._Matrix.ToMatrixString())
    interface IMatrix<'dim0, 'dim1>
    new ([<ParamArray>] data: 't list array) = 
        let array = data |> Array.map (fun a -> Array.ofList a) in
        Matrix<'dim0, 'dim1, 't>(array)
    new(x: Expr<'t list list>) = 
        let values = x |> expand_list_values<'t> |> List.map List.toArray |> List.toArray
        Matrix<'dim0,'dim1, 't>(values, x)
    static member create([<ParamArray>] data: 't array array) = Matrix<'dim0,'dim1,'t>(data)
    static member fromMNMatrix(m: LinearAlgebra.Matrix<'t>) = Matrix<'dim0, 'dim1, 't>.create(let a = m.AsRowArrays() in if isNull a then m.ToRowArrays() else a) 
    //static member (+)(l : Matrix<'n, 'm, 't>, r : Matrix<'n, 'm, 't>) = l._Matrix + r._Matrix |> Matrix<'n, 'm, 't>.fromMNMatrix
    //static member (-)(l : Matrix<'n, 'm, 't>, r : Matrix<'n, 'm, 't>) = l._Matrix - r._Matrix |> Matrix<'n, 'm, 't>.fromMNMatrix
    //static member (*)(l : Matrix<'n, 'm, 't>, r : Matrix<'m, 'p, 't>) =  l._Matrix * r._Matrix |> Matrix<'n, 'p, 't>.fromMNMatrix

type Mat<'dim0, 'dim1, 't when 'dim0 :> Number and 'dim1 :> Number and 't:> ValueType and 't : struct and 't: (new: unit -> 't) and 't :> IEquatable<'t> and 't :> IFormattable and 't :> IComparable> =
    Matrix<'dim0, 'dim1, 't>
type Mat<'dim0, 'dim1 when 'dim0 :> Number and 'dim1 :> Number> = Matrix<'dim0, 'dim1, R>
type MatF<'dim0, 'dim1 when 'dim0 :> Number and 'dim1 :> Number> = Matrix<'dim0, 'dim1, float32>