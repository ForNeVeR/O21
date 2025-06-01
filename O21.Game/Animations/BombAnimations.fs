// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.U95

module BombAnimations =

    type BombAnimation = {
        Sprites: FishSprites
        Idle: Animation
    } with
        static member Init(data: Sprites, bomb: Bomb) =
            let bombSprites = data.Bombs[bomb.Type]
            let idleSprites = bombSprites.RightDirection
            {
                Sprites = bombSprites
                Idle = Animation.Init(idleSprites, LoopTime.Infinity, 1UL, AnimationDirection.Forward)
            }

        member private this.OnDyingAnimation(tick: uint64): Animation Lazy =
            lazy {
                Frames = this.Sprites.OnDying
                LoopTime = LoopTime.Count 1
                Direction = AnimationDirection.Forward
                TicksPerFrame = 2UL
                CurrentFrame = (0, tick)
            }

        member this.Update(engine: TickEngine) =
            let tick = engine.ProcessedTicks
            
            { this with
                Idle = (this.Idle.Update tick).Value }

        member this.Draw(bomb: Bomb) =
            this.Idle.Draw(bomb.TopLeft)
