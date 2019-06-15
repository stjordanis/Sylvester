namespace Sylvester.Arithmetic.Collections

open System.Collections.Generic
open Sylvester.Arithmetic
open Sylvester.Arithmetic.N10
open System
    
module VList = 
    type VList<'n, 't when 'n: (static member Zero : N0) and 'n : (static member op_Explicit: 'n -> int)>() = 
    
        member inline x.Length = getN<'n>()
       
        member inline x._List = 
            let length = x.Length |> int
            let _list = new List<'t>(length)
            for i in 1..length do _list.Add(Unchecked.defaultof<'t>)
            _list
      
        member inline x.Item(i:'i when 'i : (static member (+<): 'i -> 'n -> True)) : 't = x._List.[i |> int]
            
        member inline x.GetSlice(start : 'a option , finish : 'b option when ('a or 'b): (static member Zero : N0) and 'a : (static member op_Explicit: 'a -> int) and 'b : (static member op_Explicit: 'b -> int)
                                                                and 'a : (static member (+<): 'a -> 'n -> True) 
                                                                and 'b : (static member (+<): 'b -> 'n -> True)) : VList<'c, 't> = 
                                           
                                                                let _start, _finish = start.Value, finish.Value 
                                                                
                                                                let intstart = _start |> int
                                                                let intfinish = _finish |> int
                                                                let count = _finish - _start
                                                                let intcount = count |> int
                                                                let inline create(n:'x when 'x: (static member Zero : N0) and 'x : (static member op_Explicit: 'x -> int)) = VList<'x,'t>()
                                                                let v = create(count)
                                                                let slice = x._List.GetRange(intstart, intfinish - 1)
                                                                v
                                                                //for i in 0 .. intcount - 1 do v.SetVal(i, )
                                                           
        member inline x.SetVal(i:'i, item:'t when 'i : (static member (+<): 'i -> 'n -> True)) = 
            x._List.Item((i |> int)) <- item

        member inline x.SetVal(i:int, item:'t) =
            if i < (x.Length |> int) then x._List.Item((i)) <- item else failwith "Index out of range."

        member inline x.At<'i when  'i : (static member Zero : N0) 
                                and 'i : (static member (+<): 'i -> 'n -> True) 
                                and 'i : (static member op_Explicit: 'i -> int)>() : 't = x.Item(getN<'i>())