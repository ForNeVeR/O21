// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game.Engine
open Raylib_CsLo

[<RequireQualifiedAccess>]
type LoopTime =
    | Count of int
    | Infinity
    
type AnimationState =
    | Playing = 0
    | PlayingReversing = 1
    | Stopped = 2
    
type Animation = {
    Frames: Texture[]
    LoopTime: LoopTime
    TicksPerFrame: int
    CurrentFrame: int * int
} with
    static member Init(frames: Texture[], loop:LoopTime, ticksPerFrame: int) =
        {
            Frames = frames
            LoopTime = loop
            TicksPerFrame = ticksPerFrame
            CurrentFrame = (0, 0)
        }
        
    member this.GetState =
        let ticks = this.TicksPerFrame
        if ticks = 0 then AnimationState.Stopped
        else if ticks > 0 then AnimationState.Playing
        else AnimationState.PlayingReversing
    
    member this.Update(currentTick: int) =
        if this.TicksPerFrame = 0 then Some this
        else
            let frame, frameTick = this.CurrentFrame
            let elapsed = (currentTick - frameTick)
            let frame, frameTick =
                if elapsed >= Math.Abs(this.TicksPerFrame) then
                    frame + Math.Clamp(this.TicksPerFrame, -1, 1), currentTick
                else
                    frame, frameTick
            match this.LoopTime with
                | LoopTime.Count count ->
                    if count > 0 then
                        if frame >= this.Frames.Length then
                            Some { this with CurrentFrame = (0, currentTick); LoopTime = LoopTime.Count(count - 1) }
                        else if frame < 0 then
                            Some { this with CurrentFrame = (this.Frames.Length - 1, currentTick); LoopTime = LoopTime.Count(count - 1) }
                        else
                            Some { this with CurrentFrame = (frame, frameTick) }
                    else None
                | LoopTime.Infinity ->
                    let frame =
                        if frame < 0 then this.Frames.Length - 1
                        else frame % this.Frames.Length
                    Some { this with CurrentFrame = (frame, frameTick) }
        
    member this.Draw(Point(x, y)) =
        let frame, _ = this.CurrentFrame
        Raylib.DrawTexture(this.Frames[Math.Abs(frame)], x, y, Raylib.WHITE)

