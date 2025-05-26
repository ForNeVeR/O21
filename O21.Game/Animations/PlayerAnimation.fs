// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game
open O21.Game.Engine
open O21.Game.U95

// TODO[#123]: Generalize player and enemies animations
type PlayerAnimation = {
    Sprites: PlayerSprites
    AnimationQueue: Animation list
    MovementAnimation: Animation
} with
    static member Init(data: U95Data) =
        {
            Sprites = data.Sprites.Player
            AnimationQueue = []
            MovementAnimation = Animation.Init(data.Sprites.Player.Right, LoopTime.Infinity, 0UL, AnimationDirection.Forward)
        }
        
    member private this.UpdateMovementAnimation(player: Player) (tick: uint64)=
        let sprites =
            match player.Direction with
                | Left -> this.Sprites.Left
                | Right -> this.Sprites.Right
        let velocity = player.Velocity.X |> (Math.Abs >> uint64)
        { this.MovementAnimation.Update(tick).Value with
            Frames = sprites
            TicksPerFrame =
                if velocity = 0UL
                    then 0UL
                    else uint64 (GameRules.MaxPlayerVelocity + 1) - velocity
            Direction =
                if player.Velocity.X < 0
                    then AnimationDirection.Backward
                    else AnimationDirection.Forward }
        
    member private this.ExplosionAnimation(tick: uint64): Animation =
        {
            Frames = this.Sprites.Explosion
            LoopTime = LoopTime.Count 1
            Direction = AnimationDirection.Forward
            TicksPerFrame = 2UL
            CurrentFrame = (0, tick)
        }

    member this.Update(state: State, animations: AnimationType[]) =
        let tick = state.Engine.ProcessedTicks
        let player = state.Engine.Game.Player
        let mutable queue =
            if this.AnimationQueue.IsEmpty then []
            else
                match this.AnimationQueue.Head.Update tick with
                | None -> this.AnimationQueue.Tail
                | Some updated -> updated :: this.AnimationQueue.Tail
            
        if Array.contains AnimationType.Die animations then
            queue <- this.ExplosionAnimation tick :: queue
            
        { this with
            AnimationQueue = queue
            MovementAnimation = this.UpdateMovementAnimation player tick }

    member this.Draw(state: State) =
        match this.AnimationQueue with
        | [] -> this.MovementAnimation.Draw(state.Engine.Game.Player.TopLeft)
        | anim :: _ -> anim.Draw(state.Engine.Game.Player.TopLeft)
