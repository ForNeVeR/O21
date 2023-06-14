namespace O21.Game.Engine

[<Struct>]
type HorizontalDirection =
    | Left
    | Right
    static member (*)(direction: HorizontalDirection, value: int) =
        match direction with
        | Left -> -value
        | Right -> value

type Player = {
    Position: Point
    Velocity: Vector
} with
    member this.Direction: HorizontalDirection =
        // TODO: Properly process zero velocity: should be possible to preserve the direction
        if this.Velocity.X < 0 then
            HorizontalDirection.Left
        else
            HorizontalDirection.Right
    member this.Update(timeDelta: int): Player =
        { this with Position = this.Position + this.Velocity * timeDelta  }

type Bullet = {
    Position: Point
    Direction: HorizontalDirection
} with
    member this.Update(timeDelta: int): Bullet option =
        let Point(x, y) as newPosition =
            this.Position +
            Vector(this.Direction * GameRules.BulletVelocity * timeDelta, 0)
        if x < 0 || x > GameRules.LevelWidth then
            None
        else
            Some { this with Position = newPosition }
