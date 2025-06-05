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
    | Stopped = 2

type AnimationDirection =
    | Forward = 0
    | Backward = 1

type Animation = {
    Frames: Texture2D[]
    LoopTime: LoopTime
    Direction: AnimationDirection
    TicksPerFrame: uint64
    CurrentFrame: int * uint64
} with
    static member Init(
        frames: Texture2D[],
        loop: LoopTime,
        ticksPerFrame: uint64,
        direction: AnimationDirection,
        ?engine: TickEngine
    ) =
        {
            Frames = frames
            LoopTime = loop
            Direction = direction
            TicksPerFrame = ticksPerFrame
            CurrentFrame = (0, engine |> Option.map _.ProcessedTicks |> Option.defaultValue 0UL)
        }
        
    member this.GetState() =
        let ticks = this.TicksPerFrame
        if ticks = 0UL then AnimationState.Stopped
        else AnimationState.Playing
    
    member this.Update(currentTick: uint64) =
        if this.TicksPerFrame = 0UL then Some this
        else
            let frame, frameTick = this.CurrentFrame
            let elapsed = (currentTick - frameTick)
            let frame, frameTick =
                if elapsed >= this.TicksPerFrame then
                    let count = elapsed / this.TicksPerFrame |> int
                    let frame =
                        match this.Direction with
                        | AnimationDirection.Forward -> frame + count
                        | AnimationDirection.Backward -> frame - count
                        | _ -> frame
                    frame, (currentTick - (elapsed % this.TicksPerFrame))
                else
                    frame, frameTick
            let loops = Math.Abs frame / this.Frames.Length |> Math.Abs
            let remains = Math.Abs frame - loops * this.Frames.Length
            match this.LoopTime with
                | LoopTime.Count count ->
                    if count > 0 then
                        if frame >= this.Frames.Length then
                            Some { this with CurrentFrame = (remains, frameTick); LoopTime = LoopTime.Count(count - loops) }
                        else if frame < 0 then
                            Some { this with CurrentFrame = (this.Frames.Length - remains - 1, frameTick); LoopTime = LoopTime.Count(count - loops) }
                        else
                            Some { this with CurrentFrame = (frame, frameTick) }
                    else None
                | LoopTime.Infinity ->
                    let frame =
                        if frame < 0 then this.Frames.Length - remains - 1
                        else remains
                    Some { this with CurrentFrame = (frame, frameTick) }
        
    member this.Draw(Point(x, y)) =
        let frame, _ = this.CurrentFrame
        DrawTexture(this.Frames[Math.Abs(frame)], x, y, Color.White)
