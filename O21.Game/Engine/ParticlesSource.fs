// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game.U95

type ParticlesSource = {
    Particles: Particle[]
    Timer: GameTimer
} with
    member this.Update(level: Level, player: Player, timeDelta: int): ParticlesSource =
        let particles = this.Particles |> Array.choose(_.Update(level, timeDelta))
        let timer = this.Timer.Update(timeDelta)
        
        if timer.HasExpired then
            let timer = timer.ResetN 1
            let particleTimeDelta = timer.TimeElapsed
            {
                Particles = particles |> Array.append ([|this.GenerateFromPlayer(particleTimeDelta, level, player)|] |> Array.choose id)
                Timer = { timer with Period = this.PickRandom GameRules.ParticlesPeriodRange }
            }
        else
            {
                Particles = particles
                Timer = timer 
            }
        
    member private this.PickRandom(range:int array) =
        let index = Random.Shared.Next(range.Length)
        range[index]
        
    member private this.GenerateFromPlayer(timeDelta: int, level: Level, player: Player) =
        let startPosition = GameRules.NewParticlePosition (player.TopForward, player.Direction)
        let offset = this.PickRandom GameRules.ParticlesOffsetRange
        let initialSpeed = -player.Velocity.Y
        let speed = GameRules.ParticleSpeed + if initialSpeed < 0 then 0 else initialSpeed
        {
            TopLeft = startPosition + Vector(offset, 0)
            Speed = speed
        }.Update(level, timeDelta)

    static member Default = {
            Particles = Array.empty
            Timer = GameTimer.Default 
        }
