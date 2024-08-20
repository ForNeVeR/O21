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
    
    member this.Update(timeDelta: int) =
        let newElapsed = this.TimeElapsed + timeDelta
        let timer = this
        { timer with TimeElapsed = newElapsed }
        
    member this.Reset =
        let newElapsed = Math.Max(this.TimeElapsed - this.Period, 0)
        let timer = this
        { timer with TimeElapsed = newElapsed }
    static member Default = {
        TimeElapsed = 0
        Period = 0 
    }
