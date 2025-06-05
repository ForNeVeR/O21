// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game
open O21.Game.Engine
open O21.Game.U95

type PlayerAnimation = {
    Sprites: PlayerSprites
    AnimationQueue: Animation list
    Movement: Animation
} with
    static member Init(data: Sprites) =
        {
            Sprites = data.Player
            AnimationQueue = []
            Movement = Animation.Init(data.Player.Right, LoopTime.Infinity, 0UL, AnimationDirection.Forward)
        }
        
    member private this.UpdateMovementAnimation(player: Player) (tick: uint64)=
        let sprites =
            match player.Direction with
                | HorizontalDirection.Left -> this.Sprites.Left
                | HorizontalDirection.Right -> this.Sprites.Right
        let velocity = player.Velocity.X |> (Math.Abs >> uint64)
        { this.Movement.Update(tick).Value with
            Frames = sprites
            TicksPerFrame =
                if velocity = 0UL
                    then 0UL
                    else Checked.uint64 (GameRules.MaxPlayerVelocity + 1) - velocity
            Direction =
                if player.Velocity.X < 0
                    then AnimationDirection.Backward
                    else AnimationDirection.Forward }
        
    member private this.ExplosionAnimation(tick: uint64): Animation Lazy =
        lazy {
            Frames = this.Sprites.Explosion
            LoopTime = LoopTime.Count 1
            Direction = AnimationDirection.Forward
            TicksPerFrame = 2UL
            CurrentFrame = (0, tick)
        }

    member this.Update(engine: TickEngine, animations: AnimationType[]) =
        let tick = engine.ProcessedTicks
        let player = engine.Game.Player
        let mutable queue =
            if this.AnimationQueue.IsEmpty then []
            else
                match this.AnimationQueue.Head.Update tick with
                | None -> this.AnimationQueue.Tail
                | Some updated -> updated :: this.AnimationQueue.Tail
            
        if Array.contains AnimationType.Die animations then
            queue <- (this.ExplosionAnimation tick).Value :: queue
            
        { this with
            AnimationQueue = queue
            Movement = this.UpdateMovementAnimation player tick }

    member this.Draw(state: State) =
        match this.AnimationQueue with
        | [] -> this.Movement.Draw(state.Engine.Game.Player.TopLeft)
        | anim :: _ -> anim.Draw(state.Engine.Game.Player.TopLeft)
