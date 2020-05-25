﻿namespace Sylvester

open System
open System.Reflection
open FSharp.Reflection
open FSharp.Quotations

open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
                
[<AutoOpen>] 
module FsExpr =
    let (!!) x = unbox x

    let rec range_type a = if FSharpType.IsFunction a then range_type(FSharpType.GetFunctionElements(a) |> snd) else a
    
    let getFieldInfo = function
    | FieldGet (_, fieldInfo) -> fieldInfo
    | _ -> failwith "Expression is not a field."

    let getPropertyInfo = function
    | PropertyGet (_, info, _) -> info
    | _ -> failwith "Expression is not a property."

    let getMethodInfo = function
    | Call (_, info, _) -> info
    | _ -> failwith "Expression is not a method."

    let rec getFuncName = function
    | Call(None, methodInfo, _) -> methodInfo.Name
    | Lambda(_, expr) -> getFuncName expr
    | _ -> failwith "Expression is not a function."

    let getFuncInfo (t:Type) expr = 
        match expr |> getFuncName |> t.GetMember |> List.ofArray with
        | [] -> None
        | _ -> t.GetMethod(getFuncName expr) |> Some

    let getModuleType = function
    | PropertyGet (_, info, _) -> info.DeclaringType
    | FieldGet (_, info) -> info.DeclaringType
    | Call (_, info, _) -> info.DeclaringType
    | _ -> failwith "Expression does not have information to retrieve module type."

    let rec getExprName = function
    | Call(None, info, _) -> info.Name
    | Lambda(_, expr) -> getExprName expr
    | PropertyGet (_, info, _) -> info.Name
    | FieldGet (_, info) -> info.Name
    | _ -> failwith "Expression does not have information to retrieve name."


    let sequal (l:Expr) (r:Expr) = 
        (l.ToString() = r.ToString()) 
        || l.ToString() = sprintf "(%s)" (r.ToString())
        || r.ToString() = sprintf "(%s)" (l.ToString())
    
    let sequal2 (l1:Expr) (l2:Expr) (r1:Expr) (r2:Expr) = sequal l1 r1 && sequal l2 r2
    
    let sequal3 (l1:Expr) (l2:Expr) (l3:Expr) (r1:Expr) (r2:Expr) (r3:Expr)= sequal l1 r1 && sequal l2 r2 && sequal l3 r3

    let vequal (lv1:Var) (lv2:Var) = lv1.Name = lv2.Name && lv1.Type = lv2.Type

    let vequal2 (lv1:Var) (lv2:Var) (rv1:Var) (rv2:Var) = vequal lv1 rv1 && vequal lv2 rv2
    
    let vequal3 (lv1:Var) (lv2:Var) (lv3:Var) (rv1:Var) (rv2:Var) (rv3:Var) = vequal lv1 rv1 && vequal lv2 rv2 && vequal lv3 rv3

    let vequal_single (lv1:Var) (lv2:Var list) = lv2.Length = 1 && vequal lv2.Head lv1

    let vequal' (lv1:Var list) (lv2:Var list) = lv1.Length = lv2.Length && List.fold2(fun s v1 v2 -> s && vequal v1 v2) true lv1 lv2

    let src expr = Swensen.Unquote.Operators.decompile expr
        
    let rec body = 
        function
        | Lambda(_, Lambda(v2, b)) -> body (Expr.Lambda(v2, b))
        | Lambda(_, b) -> b
        | Let(_, _, b) -> b
        | expr -> expr

    let traverse expr f =
        match expr with
        | ShapeVar v -> Expr.Var v
        | ShapeLambda (v, body) -> Expr.Lambda (v, f body)
        | ShapeCombination (o, exprs) -> RebuildShapeCombination (o,List.map f exprs)

    let subst_var_value (var:Var) (value: Expr) (expr:Expr)  =
        expr.Substitute(fun v -> if v.Name = var.Name && v.Type = var.Type then Some value else None)

    let subst_var_value' (lvar:Var list) (lval:Expr list) (expr:Expr)  =
        do if not (lvar.Length = lval.Length) then failwithf "The lengths of the variable/expression lists lists are not the same for replacement operation in %s." (src expr)
        List.fold2 (fun e v s -> subst_var_value v s e) expr lvar lval

    let rec replace_var_expr (var:Var) (value:Expr) (expr:Expr)  =
        match expr with
        | ShapeVar v  when vequal v var ->  value
        | expr -> traverse expr (replace_var_expr var value)

    let replace_var_expr' (lvar:Var list) (lexpr:Expr list) (expr:Expr)  =
        do if not (lvar.Length = lexpr.Length) then failwithf "The lengths of the variable/expression lists lists are not the same for replacement operation in %s." (src expr)
        List.fold2 (fun e v r -> replace_var_expr v r e) expr lvar lexpr

    let rec replace_var_var (var1:Var) (var2:Var) (expr:Expr)  =
        match expr with
        | ShapeVar v  when vequal v var1-> Expr.Var var2
        | expr -> traverse expr (replace_var_var var1 var2)

    let replace_var_var' (lvar1:Var list) (lvar2:Var list) (expr:Expr)  =
        do if not (lvar1.Length = lvar2.Length) then failwithf "The lengths of the variable lists are not the same for replacement operation in %s." (src expr)
        List.fold2 (fun e v1 v2 -> replace_var_var v1 v2 e) expr lvar1 lvar2
       
    let subst_all_value (expr:Expr) (value:Expr) = expr.Substitute(fun _ -> Some value)
        
    let get_vars expr =
        let rec rget_vars prev expr =
            match expr with
            | ShapeVar v -> prev @ [v]
            | ShapeLambda (v, body) -> rget_vars (prev @ [v]) body
            | ShapeCombination (_, exprs) ->  List.map (rget_vars prev) exprs |> List.collect id
            
        rget_vars [] expr |> List.distinctBy (fun v -> v.Name)

    let get_var_names expr = get_vars expr |> List.map (fun v -> v.Name)
    
    let occurs (var:Var list) (expr:Expr) = 
        expr |> get_vars |> List.exists(fun v -> List.exists(fun vv -> vequal v vv) var)

    let not_occurs (var:Var list) (expr:Expr) = not (occurs var expr)

    let vars_to_tuple (vars:Var list) = vars |> List.map (fun v -> Expr.Var v) |> Expr.NewTuple
    
    /// Based on: http://www.fssnip.net/bx/title/Expanding-quotations by Tomas Petricek.
    /// Expand variables and calls to methods and propery getters.
    let expand expr =
        let rec rexpand vars expr = 
          let expanded = 
            match expr with
            | WithValue(_, _, e) -> rexpand vars e
            | Coerce(e, _) -> rexpand vars e
            | ValueWithName(o, t, n) -> rexpand vars (Expr.Value(o, t))
            | Call(body, MethodWithReflectedDefinition meth, args) ->
                let this = match body with Some b -> Expr.Application(meth, b) | _ -> meth
                let res = Expr.Applications(this, [ for a in args -> [a]])
                rexpand vars res
            | PropertyGet(body, PropertyGetterWithReflectedDefinition p, []) -> 
                let this = match body with Some b -> b | None -> p
                rexpand vars this
            | PropertyGet(None, p, []) -> rexpand vars (Expr.Var(Var(p.Name, p.PropertyType)))
            // If the variable has an assignment, then replace it with the expression
            | ExprShape.ShapeVar v when Map.containsKey v vars -> vars.[v]
            // Else apply rexpand recursively on all sub-expressions
            | _ -> traverse expr (rexpand vars)
          // After expanding, try reducing the expression - we can replace 'let' expressions and applications where the first argument is lambda.
          match expanded with
          | Application(ExprShape.ShapeLambda(v, body), assign)
          | Let(v, assign, body) ->
                rexpand (Map.add v (rexpand vars assign) vars) body
          | _ -> expanded

        rexpand Map.empty expr

    let expand_left = 
        function
        | Call(_,_,l::r::[]) when l.Type = r.Type -> expand l 
        | expr -> failwithf "%s is not a binary expression." (src expr)

    let expand_right = 
        function
        | Call(_,_,l::r::[]) when l.Type = r.Type -> expand r
        | expr -> failwithf "%s is not a binary expression." (src expr)

    let expandReflectedDefinitionParam = 
        function
        | WithValue(v, t, e) -> (v, t, expand e)
        | _ -> failwith "Expression is not a reflected definition parameter."

    (*
    let expand_list =
        let rec rexpand_list (e:Expr list) =
            function
            | NewUnionCase(uc, Var v) -> e @ [v]
      *)      
    let binary_call (so, m, l, r) =
        match so with
        | None -> Expr.Call(m, l::r::[])
        | Some o -> Expr.Call(o, m, l::r::[])

    let unary_call (so, m, l) =
        match so with
        | None -> Expr.Call(m, [l])
        | Some o -> Expr.Call(o, m, [l])
