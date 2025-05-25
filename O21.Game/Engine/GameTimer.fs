// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine


[<Struct>]
type GameTimer = {
    TimeElapsed: int
    Period: int
} with
    member this.HasExpired = this.TimeElapsed >= this.Period

    member this.Tick() =
        let newElapsed = this.TimeElapsed + 1
        let timer = this
        { timer with TimeElapsed = newElapsed }

    member this.Reset() =
        let timer = this
        { timer with TimeElapsed = 0 }
        
    static member Default = {
        TimeElapsed = 0
        Period = 0 
    }
