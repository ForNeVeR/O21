namespace O21.Game.Engine

open System

type ParticlesSource = {
    TimeElapsed: int
    Particles: Particle[]
    Period: int
} with
    member this.Update(timeDelta: int) (player:Player): ParticlesSource =
        let newElapsed = this.TimeElapsed + timeDelta
        let particles = this.Particles |> Array.choose(_.Update(timeDelta))
        
        if newElapsed >= this.Period then
            {
                TimeElapsed = newElapsed - this.Period
                Particles = particles |> Array.append ([|(this.GenerateFromPlayer player)|] |> Array.choose id)
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
        
    member private this.GenerateFromPlayer(player:Player) : Particle option =
        let startPosition = GameRules.NewParticlePosition (player.TopForward, player.Direction)
        let offset = this.PickRandom GameRules.ParticlesOffsetRange
        { Position = startPosition + Vector(offset, 0) }.Update 0
