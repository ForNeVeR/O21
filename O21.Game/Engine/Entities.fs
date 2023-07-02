namespace O21.Game.Engine

type Player = {
    TopLeft: Point
    Velocity: Vector
    PrevDirection: HorizontalDirection
    ShotCooldown: int
} with

    member this.TopRight = this.TopLeft + Vector(GameRules.PlayerSize.X, 0)

    member this.Direction: HorizontalDirection =
        // TODO[#129]: Properly process zero velocity: should be possible to preserve the 
            if this.Velocity.X < 0 then
                HorizontalDirection.Left
            else if this.Velocity.X > 0 then
                HorizontalDirection.Right
            else
                this.PrevDirection

    member this.IsAllowedToShoot = this.ShotCooldown = 0

    /// The coordinate of top corner of the forward side (i.e. the one it's directed at) of the sprite.
    member this.TopForward =
        match this.Direction with
        | HorizontalDirection.Left -> this.TopLeft
        | HorizontalDirection.Right -> this.TopRight

    member this.Update(timeDelta: int): Player =
        { this with
            TopLeft = this.TopLeft + this.Velocity * timeDelta
            ShotCooldown = max (this.ShotCooldown - timeDelta) 0
            PrevDirection = this.Direction
        }

type Bullet = {
    Position: Point
    Direction: HorizontalDirection
} with
    member this.Update(timeDelta: int): Bullet option =
        let Point(x, _) as newPosition =
            this.Position +
            Vector(this.Direction * GameRules.BulletVelocity * timeDelta, 0)
        if x < 0 || x > GameRules.LevelWidth then
            None
        else
            Some { this with Position = newPosition }
