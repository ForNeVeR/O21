// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Engine.GameRules

open System
open O21.Game.U95

[<Literal>]
let GameScreenWidth = 600

[<Literal>]
let GameScreenHeight = 400

[<Literal>]
let ShotCooldownTicks = 15

[<Literal>]
let LevelWidth = GameScreenWidth

[<Literal>]
let LevelHeight = 300

[<Literal>]
let TicksPerSecond = 10.0

let MaxPlayerVelocity = 3
let ClampVelocity(Vector(x, y)): Vector =
   Vector(
       Math.Clamp(x, -MaxPlayerVelocity, MaxPlayerVelocity),
       Math.Clamp(y, -MaxPlayerVelocity, MaxPlayerVelocity)
   )

[<Literal>]
let BulletVelocity = 5 // TODO[#131]: Compare with the original

[<Literal>]
let BulletLifetime = 10

[<Literal>]
let ParticleSpeed = 3

let ParticlesPeriodRange = [|7..10|]
let ParticlesOffsetRange = [|-2..2|]

let BrickSize = Vector(12, 12)
let PlayerSize = Vector(46, 27)
let BulletSize = Vector(6, 6)
let ParticleSize = Vector(5, 5)

/// Relative position of the bullet sprite's top left corner to the player sprite's top corner (from the shooting side)
/// when the bullet appears.
let NewBulletPosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(-4 - BulletSize.X, 14)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(4, 14)
    
let StartingLevel = LevelCoordinates(1, 1)
let PlayerStartingPosition = Point(200, 140)
let LevelSizeInTiles = Vector(50, 25)

let NewParticlePosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(28, -ParticleSize.Y)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(-28, -ParticleSize.Y)
