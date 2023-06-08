namespace O21.Game.Engine

type Time = {
    Total: float
    Delta: float32
}

type GameEngine = {
    StartTime: Time
    Tick: int
    Player: Player
} with
    static member Start(time: Time): GameEngine = {
        StartTime = time
        Tick = 0
        Player = {
            Position = Point(0, 0)
            Velocity = Vector(0, 0)
        }
    }

    member this.Update(time: Time): GameEngine =
        let newTick = int <| (time.Total - this.StartTime.Total) * GameRules.TicksPerSecond
        { this with
            Tick = newTick
            Player = this.Player.Update(newTick - this.Tick)
        }

    member this.ApplyCommand(command: Command): GameEngine =
        match command with
        | VelocityDelta(delta) ->
            { this with
                Player =
                    { this.Player with
                        Velocity = GameRules.ClampVelocity(this.Player.Velocity + delta)
                    }
            }
