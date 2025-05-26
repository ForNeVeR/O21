// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game.Engine
open Raylib_CSharp.Colors
open Raylib_CSharp.Textures
open type Raylib_CSharp.Rendering.Graphics

[<RequireQualifiedAccess>]
type LoopTime =
    | Count of int
    | Infinity
    
type AnimationState =
    | Playing = 0
    | PlayingReversing = 1
    | Stopped = 2

[<RequireQualifiedAccess>]
type AnimationDirection =
    | Forward
    | Backward

type Animation = {
    Frames: Texture2D[]
    LoopTime: LoopTime
    Direction: AnimationDirection
    TicksPerFrame: uint64
    CurrentFrame: int * uint64
} with
    static member Init(frames: Texture2D[], loop: LoopTime, ticksPerFrame: uint64, direction: AnimationDirection) =
        {
            Frames = frames
            LoopTime = loop
            Direction = direction
            TicksPerFrame = ticksPerFrame
            CurrentFrame = (0, 0UL)
        }
        
    member this.GetState =
        let ticks = this.TicksPerFrame
        if ticks = 0UL then AnimationState.Stopped
        else if ticks > 0UL then AnimationState.Playing
        else AnimationState.PlayingReversing
    
    member this.Update(currentTick: uint64) =
        if this.TicksPerFrame = 0UL then Some this
        else
            let frame, frameTick = this.CurrentFrame
            let elapsed = (currentTick - frameTick)
            let frame, frameTick =
                if elapsed >= this.TicksPerFrame then
                    let frame =
                        if frame = this.Frames.Length then
                            match this.Direction with
                            | AnimationDirection.Forward -> 0
                            | AnimationDirection.Backward -> frame - 1
                        elif frame = -1 then
                            match this.Direction with
                            | AnimationDirection.Backward -> this.Frames.Length - 1
                            | AnimationDirection.Forward -> frame + 1
                        else
                            match this.Direction with
                            | AnimationDirection.Forward -> frame + 1
                            | AnimationDirection.Backward -> frame - 1
                    frame, currentTick
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
        DrawTexture(this.Frames[Math.Abs(frame)], x, y, Color.White)
