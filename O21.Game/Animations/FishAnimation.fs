// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.Engine.Enemies
open O21.Game.U95
open O21.Game.U95.Fish

type FishAnimation = {
    Sprites: FishSprites
    AnimationQueue: Animation list
    Movement: Animation
} with
    static member private GetMovementSprites (sprites: FishSprites) (fish: Fish) =
        match fish.Direction with
        | Left -> sprites.LeftDirection
        | Right -> sprites.RightDirection
    
    static member Init(data: U95Data, fish: Fish) =
        let fishId = fish.Type % data.Sprites.Fishes.Length
        let fishSprites = data.Sprites.Fishes[fishId]
        let movementSprites = FishAnimation.GetMovementSprites fishSprites fish
        {
            Sprites = fishSprites
            AnimationQueue = []
            Movement = Animation.Init(movementSprites, LoopTime.Infinity, 0UL, AnimationDirection.Forward)
        }

    member private this.UpdateMovementAnimation(fish: Fish) (tick: uint64) =
        let sprites = FishAnimation.GetMovementSprites this.Sprites fish
        { this.Movement.Update(tick).Value with
            Frames = sprites
            TicksPerFrame = 2UL }

    member private this.OnDyingAnimation(tick: uint64): Animation Lazy =
        lazy {
            Frames = this.Sprites.OnDying
            LoopTime = LoopTime.Count 1
            Direction = AnimationDirection.Forward
            TicksPerFrame = 2UL
            CurrentFrame = (0, tick)
        }

    member this.Update(state: State, fish: Fish, animations: AnimationType[]) =
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
            AnimationQueue = queue
            Movement = this.UpdateMovementAnimation fish tick }

    member this.Draw(fish: Fish) =
        match this.AnimationQueue with
        | [] -> this.Movement.Draw(fish.TopLeft)
        | anim :: _ -> anim.Draw(fish.TopLeft)
