// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.U95
open Raylib_CSharp.Textures

type LifebuoyAnimation = {
    Sprites: Texture2D[]
    Idle: Animation
} with
    static member private SetCorrectSequence (sprites: Texture2D[]) =
        Array.collect id [|
            Array.take 2 sprites
            Array.skip 4 sprites |> Array.take 8
            Array.skip 2 sprites |> Array.take 2
        |]
    
    static member Init(data: Sprites) = 
        let sprites =
            data.Bonuses.Lifebuoy
            |> LifebuoyAnimation.SetCorrectSequence
        {
            Sprites = sprites
            Idle = Animation.Init(sprites, LoopTime.Infinity, 1UL, AnimationDirection.Forward)
        }

    member this.Update(engine: TickEngine) =
        let tick = engine.ProcessedTicks
        { this with
            Idle = (this.Idle.Update tick).Value }

    member this.Draw(bonus: Bonus) =
        this.Idle.Draw(bonus.TopLeft)
