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
} with
    static member Start(time: Time): GameEngine = {
        StartTime = time
        Tick = 0
        Player = {
            TopLeft = Point(0, 0)
            Velocity = Vector(0, 0)
            ShotCooldown = 0
            PrevDirection = Left
        }
        Bullets = Array.empty
    }

    member this.Update(time: Time): GameEngine =
        let newTick = int <| (time.Total - this.StartTime.Total) * GameRules.TicksPerSecond
        // TODO[#26]: Bullet collisions
        { this with
            Tick = newTick
            Player = this.Player.Update(newTick - this.Tick)
            Bullets = this.Bullets |> Array.choose(fun bullet -> bullet.Update(newTick - this.Tick))
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
