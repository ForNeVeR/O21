// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.Engine.Enemies
open O21.Game.U95
open O21.Game.U95.Fish

type BombAnimation = {
    Sprites: FishSprites
    AnimationQueue: Animation list
    Idle: Animation
} with
    static member Init(data: U95Data, bomb: Bomb) =
        let bombId = bomb.Id % data.Sprites.Fishes.Length
        let bombSprites = data.Sprites.Bombs[bombId]
        let idleSprites = bombSprites.RightDirection
        {
            Sprites = bombSprites
            AnimationQueue = []
            Idle = Animation.Init(idleSprites, LoopTime.Infinity, 0UL, AnimationDirection.Forward)
        }

    member private this.OnDyingAnimation(tick: uint64): Animation Lazy =
        lazy {
            Frames = this.Sprites.OnDying
            LoopTime = LoopTime.Count 1
            Direction = AnimationDirection.Forward
            TicksPerFrame = 2UL
            CurrentFrame = (0, tick)
        }

    member this.Update(state: State, animations: AnimationType[]) =
        let tick = state.Engine.ProcessedTicks
        let mutable queue =
            if this.AnimationQueue.IsEmpty then []
            else
                match this.AnimationQueue.Head.Update tick with
                | None -> this.AnimationQueue.Tail
                | Some updated -> updated :: this.AnimationQueue.Tail

        if Array.contains AnimationType.Die animations then
            queue <- (this.OnDyingAnimation tick).Value :: queue
        
        { this with
            AnimationQueue = queue }

    member this.Draw(bomb: Bomb) =
        match this.AnimationQueue with
        | [] -> this.Idle.Draw(bomb.TopLeft)
        | anim :: _ -> anim.Draw(bomb.TopLeft)
