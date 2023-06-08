namespace O21.Game.Engine

type Player = {
    Position: Point
    Velocity: Vector
} with
    member this.Update (timeDelta: int) =
        { this with Position = this.Position + this.Velocity * timeDelta  }
