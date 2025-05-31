// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open O21.Game
open O21.Game.Engine
open O21.Game.U95
open O21.Game.U95.Fish
open Raylib_CSharp.Textures

type LifebuoyAnimation = {
    Sprites: Texture2D[]
    AnimationQueue: Animation list
    Idle: Animation
} with
    static member Init(data: U95Data) = 
        let sprites = data.Sprites.Bonuses.Lifebuoy
        {
            Sprites = sprites
            AnimationQueue = []
            Idle = Animation.Init(sprites, LoopTime.Infinity, 0UL, AnimationDirection.Forward)
        }

    member this.Update(state: State, animations: AnimationType[]) =
        let tick = state.Engine.ProcessedTicks
        let mutable queue =
            if this.AnimationQueue.IsEmpty then []
            else
                match this.AnimationQueue.Head.Update tick with
                | None -> this.AnimationQueue.Tail
                | Some updated -> updated :: this.AnimationQueue.Tail
        
        { this with
            AnimationQueue = queue }

    member this.Draw(bonus: Bonus) =
        match this.AnimationQueue with
        | [] -> this.Idle.Draw(bonus.TopLeft)
        | anim :: _ -> anim.Draw(bonus.TopLeft)
