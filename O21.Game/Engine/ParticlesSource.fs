// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game.U95

type ParticlesSource = {
    Particles: Particle[]
    Timer: GameTimer
} with
    member this.Tick(level: Level, player: Player, random: ReproducibleRandom): ParticlesSource =
        let particles = this.Particles |> Array.choose(_.Tick(level))
        let timer = this.Timer.Tick()
        
        if timer.HasExpired then
            let timer = timer.Reset()
            {
                Particles = particles |> Array.append ([|this.GenerateFromPlayer(player, random)|] |> Array.choose id)
                Timer = { timer with Period = this.PickRandom(GameRules.ParticlesPeriodRange, random) }
            }
        else
            {
                Particles = particles
                Timer = timer 
            }
        
    member private this.PickRandom(range: int array, random: ReproducibleRandom) =
        let index = random.NextExcluding(range.Length)
        range[index]
        
    member private this.GenerateFromPlayer(player: Player, random: ReproducibleRandom) =
        let startPosition = GameRules.NewParticlePosition (player.TopForward, player.Direction)
        let offset = this.PickRandom(GameRules.ParticlesPeriodRange, random)
        let initialSpeed = -player.Velocity.Y
        let speed = GameRules.ParticleSpeed + if initialSpeed < 0 then 0 else initialSpeed
        Some {
            TopLeft = startPosition + Vector(offset, 0)
            Speed = speed
        }

    static member Default = {
            Particles = Array.empty
            Timer = GameTimer.Default 
        }
