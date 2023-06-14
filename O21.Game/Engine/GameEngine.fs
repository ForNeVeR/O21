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
            Position = Point(0, 0)
            Velocity = Vector(0, 0)
        }
        Bullets = Array.empty
    }

    member this.Update(time: Time): GameEngine =
        let newTick = int <| (time.Total - this.StartTime.Total) * GameRules.TicksPerSecond
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
            // TODO: The rules when it's allowed to shoot and when it isn't
            // TODO: Proper bullet position
            let newBullet = {
                Position = this.Player.Position
                Direction = this.Player.Direction
            }
            { this with
                Bullets = Array.append this.Bullets [| newBullet |] // TODO: Make more efficient (persistent vector?)
            }, [| PlaySound SoundType.Shot |]
