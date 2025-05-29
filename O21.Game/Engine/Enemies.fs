// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game.Engine.Environments
open O21.Game.Engine.Geometry

[<RequireQualifiedAccess>]
type EnemyEffect<'e> =
    | Update of 'e
    | PlayerHit of id: int
    | Die

type Fish = {
    TopLeft: Point
    Type: int
    Velocity: Vector
    Direction: HorizontalDirection
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.FishSizes[this.Type] }

    member this.Update(fishEnv: EnemyEnv, timeDelta: int): Fish EnemyEffect = // TODO[#27]: Fish behavior
        EnemyEffect.Die

    static member Default = {
        TopLeft = Point(0, 0)
        Type = 0
        Velocity = Vector(0, 0)
        Direction = HorizontalDirection.Right
    }

type Bomb = {
    Id: int
    TopLeft: Point
    State: BombState
} with
    static member Create(position: Point) =
        {
            Id = Random.Shared.Next(1, 1000000)
            TopLeft = position
            State = BombState.Sleep(VerticalTrigger(position.X + GameRules.BombTriggerOffset))
        }

    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.BombSize }

    member this.Tick(bombEnv: EnemyEnv): Bomb EnemyEffect =
        let level = bombEnv.Level
        let player = bombEnv.PlayerCollider
        let bullets = bombEnv.BulletColliders

        let allEntities = Array.append [|player|] bullets

        match this.State with
        | BombState.Sleep trigger ->
            let updated =
                if IsTriggered trigger player then
                    { this with State = BombState.Active(Vector(0, GameRules.BombVelocity)) }
                else
                    this
            match CheckCollision level updated.Box allEntities with
            | Collision.CollidesObject _ -> EnemyEffect.PlayerHit this.Id
            | Collision.None -> EnemyEffect.Update updated
            | _ -> EnemyEffect.Die
        | BombState.Active velocity ->
            let newBomb =
                { this with
                    TopLeft = this.TopLeft + velocity }
            match CheckCollision level this.Box allEntities with
            | Collision.CollidesObject _ -> EnemyEffect.PlayerHit this.Id
            | Collision.None -> EnemyEffect.Update newBomb
            | _ -> EnemyEffect.Die

and [<RequireQualifiedAccess>] BombState =
    | Sleep of trigger: Trigger
    | Active of velocity: Vector
