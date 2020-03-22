﻿namespace Sylvester

open FSharp.Quotations

/// Propositional calculus using the axioms and rules of S.
module PropCalculus =
    let prop_calculus = Theory.S

    /// Reduce logical constants in expression. 
    let Reduce = S.Rules.[0]

    /// Logical expression is left associative.
    let LeftAssoc = S.Rules.[1]

    /// Logical expression is right associative.
    let RightAssoc = S.Rules.[2]
  
    /// Logical expression is commutative.
    let Commute = S.Rules.[3]

    /// Distribute logical terms in expression.
    let Distrib = S.Rules.[4]

    /// Collect distributed logical terms in expression.
    let Collect = S.Rules.[5]

    /// Logical operators are idempotent.
    let Idemp = S.Rules.[6]

    /// Logical expression satisfies law of excluded middle.
    let ExcludedMiddle = Theory.S.Rules.[7]

    /// Logical expression satisfies golden rule.
    let GoldenRule = Theory.S.Rules.[8]

    // Additional theorems of S useful in proofs.

    /// not p = q = p = not q
    let NotEquivSymmetry (p:Expr<bool>) (q:Expr<bool>) = 
        let t = <@ not %p = %q = %p = not %q @> |> theorem S [
            Collect |> LeftA
            RightAssoc |> EntireA
            Commute |> RightA
            Collect |> RightA
            Commute |> RightA
        ] 
        Lemma t
    
    // not not p == p
    let DoubleNegation (p:Expr<bool>) (q:Expr<bool>) = 
        let t = <@ (%p <> %q) = not %p = %q @> |> logical_theorem [
                RightAssoc |> EntireA
                Collect |> EntireA
            ]
        Lemma t
