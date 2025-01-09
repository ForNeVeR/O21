// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game
open O21.Game.Engine
open O21.Game.U95

type AnimationHandler = {
    SubmarineAnimation: PlayerAnimation
} with
    static member Init(data: U95Data) =
        {
            SubmarineAnimation = PlayerAnimation.Init data
        }

    member this.Update(state: State, effects: ExternalEffect[]) =
        let extractAnim entityType =
            effects
            |> Array.choose (function
                | PlayAnimation (anim, t) when t = entityType -> Some anim
                | _ -> None)
            
        let playerAnims = extractAnim EntityType.Player
            
        { this with SubmarineAnimation = this.SubmarineAnimation.Update(state, playerAnims) }

    member this.Draw(state:State) =
        this.SubmarineAnimation.Draw(state)
