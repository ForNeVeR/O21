// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game.U95

type ParticlesSource = {
    TimeElapsed: int
    Particles: Particle[]
    Period: int
} with
    member this.Update(level: Level, player: Player, timeDelta: int): ParticlesSource =
        let newElapsed = this.TimeElapsed + timeDelta
        let particles = this.Particles |> Array.choose(_.Update(level, timeDelta))
        
        if newElapsed >= this.Period then
            {
                TimeElapsed = newElapsed - this.Period
                Particles = particles |> Array.append ([|this.GenerateFromPlayer(level, player)|] |> Array.choose id)
                Period = this.PickRandom GameRules.ParticlesPeriodRange 
            }
        else
            { this with
                TimeElapsed = newElapsed
                Particles = particles
            }
        
    member private this.PickRandom(range:int array) =
        let index = Random.Shared.Next(range.Length)
        range[index]
        
    member private this.GenerateFromPlayer(level: Level, player: Player) =
        let startPosition = GameRules.NewParticlePosition (player.TopForward, player.Direction)
        let offset = this.PickRandom GameRules.ParticlesOffsetRange
        { TopLeft = startPosition + Vector(offset, 0) }.Update(level, 0)
