// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System

[<Struct>]
type GameTimer = {
    TimeElapsed: int
    Period: int
} with
    member this.HasExpired = this.TimeElapsed >= this.Period
    member this.ExpirationCount =
        if this.Period > 0 then this.TimeElapsed / this.Period
        else 0
    
    member this.Update(timeDelta: int) =
        let newElapsed = this.TimeElapsed + timeDelta
        let timer = this
        { timer with TimeElapsed = newElapsed }
        
    member this.Reset = this.ResetN this.ExpirationCount
        
    member this.ResetN n =
        let expirationReset = Math.Min(n, this.ExpirationCount)
        let newElapsed = Math.Max(this.TimeElapsed - this.Period * expirationReset, 0)
        let timer = this
        { timer with TimeElapsed = newElapsed }
    static member Default = {
        TimeElapsed = 0
        Period = 0 
    }
