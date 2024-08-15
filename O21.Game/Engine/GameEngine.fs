// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open O21.Game.U95

type Time = {
    Total: float
    Delta: float32
}

type GameEngine = {
    StartTime: Time
    Tick: int
    Player: Player
    Bullets: Bullet[]
    ParticlesSource: ParticlesSource
} with
    static member Start(time: Time): GameEngine = {
        StartTime = time
        Tick = 0
        Player = {
            TopLeft = Point(0, 0)
            Velocity = Vector(0, 0)
            ShotCooldown = 0
            Direction = Right
        }
        Bullets = Array.empty
        ParticlesSource = {
            TimeElapsed = 0
            Particles = Array.empty
            Period = 0
        }
    }

    member this.Update(time: Time): GameEngine =
        let newTick = int <| (time.Total - this.StartTime.Total) * GameRules.TicksPerSecond
        let timeDelta = newTick - this.Tick
        // TODO[#26]: Bullet collisions
        { this with
            Tick = newTick
            Player = this.Player.Update(timeDelta)
            Bullets = this.Bullets |> Array.choose(_.Update(timeDelta))
            ParticlesSource = this.ParticlesSource.Update timeDelta this.Player
        }

    member this.ApplyCommand(command: PlayerCommand): GameEngine * ExternalEffect[] =
        match command with
        | VelocityDelta(delta) ->
            { this with
                Player =
                    { this.Player with
                        Velocity = GameRules.ClampVelocity(this.Player.Velocity + delta)
                    }
            }, Array.empty

        | Shoot ->
            let player = this.Player
            if player.IsAllowedToShoot then
                let newBullet = {
                    Position = GameRules.NewBulletPosition(player.TopForward, player.Direction)
                    Direction = player.Direction
                }
                { this with
                    Player = { player with ShotCooldown = GameRules.ShotCooldownTicks }
                    Bullets = Array.append this.Bullets [| newBullet |] // TODO[#130]: Make more efficient (persistent vector?)
                }, [| PlaySound SoundType.Shot |]
            else this, Array.empty
