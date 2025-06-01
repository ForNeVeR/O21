// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.U95
open Raylib_CSharp.Textures

module FishAnimations =

    type FishAnimation = {
        Sprites: FishSprites
        Movement: Animation
    } with
        static member private GetMovementSprites (sprites: FishSprites) (fish: Fish) =
            match fish.Direction with
            | HorizontalDirection.Left -> sprites.LeftDirection
            | HorizontalDirection.Right -> sprites.RightDirection
        
        static member Init(data: Sprites, fish: Fish) =
            let fishSprites = data.Fishes[fish.Type]
            let movementSprites = FishAnimation.GetMovementSprites fishSprites fish
            {
                Sprites = fishSprites
                Movement = Animation.Init(movementSprites, LoopTime.Infinity, 1UL, AnimationDirection.Forward)
            }

        member private this.UpdateMovementAnimation(fish: Fish) (tick: uint64) =
            let sprites = FishAnimation.GetMovementSprites this.Sprites fish
            { this.Movement.Update(tick).Value with
                Frames = sprites
                TicksPerFrame = 1UL }

        member this.Update(engine: TickEngine, fish: Fish) =
            let tick = engine.ProcessedTicks
            
            { this with
                Movement = this.UpdateMovementAnimation fish tick }

        member this.Draw(fish: Fish) =
            this.Movement.Draw(fish.TopLeft)
            
