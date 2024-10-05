namespace O21.Game.Animations

open System
open O21.Game.Engine
open Raylib_CsLo

[<RequireQualifiedAccess>]
type LoopTime =
    | Count of int
    | Infinity
    
type Animation = {
    Frames: Texture[]
    LoopTime: LoopTime
    TicksPerFrame: int
    CurrentFrame: int * int
} with
    member this.Update(currentTick: int) =
        if this.TicksPerFrame < 0 then Some this
        else
            let frame, frameTick = this.CurrentFrame
            let elapsed = (currentTick - frameTick)
            let frame, frameTick =
                if elapsed >= this.TicksPerFrame then
                    frame + 1, currentTick
                else
                    frame, frameTick
            match this.LoopTime with
                | LoopTime.Count count ->
                    if count > 0 then
                        if frame >= this.Frames.Length then
                            Some { this with CurrentFrame = (0, currentTick); LoopTime = LoopTime.Count(count - 1) }
                        else
                            Some { this with CurrentFrame = (frame, frameTick) }
                    else None
                | LoopTime.Infinity ->
                    let frame = frame % this.Frames.Length
                    Some { this with CurrentFrame = (frame, frameTick) }
        
    member this.Draw (Point(x, y)) =
        let frame, _ = this.CurrentFrame
        Raylib.DrawTexture(this.Frames[frame], x, y, Raylib.WHITE)

