// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open O21.Game.U95
open O21.Game.Engine.Geometry

type Player = {
    TopLeft: Point
    Velocity: Vector
    Direction: HorizontalDirection
    ShotCooldown: int
} with

    member this.TopRight = this.TopLeft + Vector(GameRules.PlayerSize.X, 0)

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
        }

type Bullet = {
    TopLeft: Point
    Direction: HorizontalDirection
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.BulletSize }
    
    member this.Update(level: Level, timeDelta: int): Bullet option =
        // Check each intermediate position of the bullet for collision:
        let maxTimeToProcessInOneStep = GameRules.BrickSize.X / GameRules.BulletVelocity
        if maxTimeToProcessInOneStep <= 0 then failwith "maxTimeToProcessInOneStep <= 0"

        if timeDelta <= maxTimeToProcessInOneStep then
            let newTopLeft =
                this.TopLeft +
                Vector(this.Direction * GameRules.BulletVelocity * timeDelta, 0)
            let newBullet = { this with TopLeft = newTopLeft }
            match CheckCollision level newBullet.Box with
            | Collision.OutOfBounds -> None
            | Collision.TouchesBrick -> None
            | Collision.None -> Some newBullet
        else
            this.Update(level, maxTimeToProcessInOneStep)
            |> Option.bind _.Update(level, timeDelta - maxTimeToProcessInOneStep)

type Particle = {
    TopLeft: Point
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.ParticleSize }
    
    member this.Update(level: Level, timeDelta: int): Particle option =
        let newPosition =
            this.TopLeft +
            Vector(0, VerticalDirection.Up * GameRules.ParticleVelocity * timeDelta)
        let newParticle = { this with TopLeft = newPosition }
        match CheckCollision level newParticle.Box with
        | Collision.OutOfBounds -> None
        | Collision.TouchesBrick -> None
        | Collision.None -> Some newParticle
