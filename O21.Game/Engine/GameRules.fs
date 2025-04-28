// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Engine.GameRules

open System
open O21.Game.U95

// ----------------------- Screen Constants -----------------------

[<Literal>]
let GameScreenWidth = 600

[<Literal>]
let GameScreenHeight = 400

// ----------------------- Player Constants -----------------------

[<Literal>]
let ShotCooldownTicks = 15

[<Literal>]
let ShotCooldownTicksWithZeroCooldownAbility = 3

[<Literal>]
let MaxPlayerLives = 9

[<Literal>]
let InitialPlayerLives = 5

[<Literal>]
let MaxPlayerVelocity = 3

[<Literal>]
let MaxOxygenUnits = 100

// ----------------------- Level Constants -----------------------

[<Literal>]
let LevelWidth = GameScreenWidth

[<Literal>]
let LevelHeight = 300

// ----------------------- Time Constants -----------------------

[<Literal>]
let TicksPerSecond = 10.0

[<Literal>]
let PostDeathFreezeTicks = 6

[<Literal>]
let OxygenUnitPeriod = 22

[<Literal>]
let BulletLifetime = 10

/// 7 Min
[<Literal>]
let AbilityLifetime = 4200

// ----------------------- Score Constants -----------------------

[<Literal>]
let GivePointsForStaticBonus = 25

[<Literal>]
let GivePointsForLifebuoy = 50

[<Literal>]
let GivePointsForBomb = 20

[<Literal>]
let GivePointsForFish = 10

[<Literal>]
let SubtractPointsForShotBonus = 10

[<Literal>]
let SubtractPointsForShot = 1

[<Literal>]
let SubtractPointsForShotByBlueBullet = 5

// ----------------------- Physics Constants -----------------------

[<Literal>]
let BulletVelocity = 5 // TODO[#131]: Compare with the original

let BulletVerticalSpread = Vector(0, 3) // TODO: Compare with the original

[<Literal>]
let BombVelocity = -5

[<Literal>]
let ParticleSpeed = 3

let ParticlesPeriodRange = [|7..10|]
let ParticlesOffsetRange = [|-2..2|]

// ----------------------- Object Sizes -----------------------

let BrickSize = Vector(12, 12)
let PlayerSize = Vector(46, 27)
let BulletSize = Vector(6, 6)
let ParticleSize = Vector(5, 5)
let FishSizes = [|Vector(25, 25); Vector(25, 25); Vector(25, 25); Vector(25, 25); Vector(25, 25)|]
let BombSize = Vector(20, 20)
let BonusSize = BombSize

// ----------------------- Functions -----------------------

/// Relative position of the bullet sprite's top left corner to the player sprite's top corner (from the shooting side)
/// when the bullet appears.
let NewBulletPosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(-4 - BulletSize.X, 14)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(4, 14)
    
let NewParticlePosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(28, -ParticleSize.Y)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(-28, -ParticleSize.Y)
    
let ClampVelocity(Vector(x, y)): Vector =
   Vector(
       Math.Clamp(x, -MaxPlayerVelocity, MaxPlayerVelocity),
       Math.Clamp(y, -MaxPlayerVelocity, MaxPlayerVelocity)
   )
   
let IsEventOccurs chance =
    Random.Shared.NextDouble() <= chance
    
let GetLevelPosition level (coordinates: int * int)  =
            Point(LevelWidth / level.LevelMap[0].Length * fst coordinates,
                  LevelHeight / level.LevelMap.Length * snd coordinates)

// ----------------------- Other Game Constants -----------------------

let StartingLevel = LevelCoordinates(1, 1)
let PlayerStartingPosition = Point(200, 140)
let LevelSizeInTiles = Vector(50, 25)
let BombTriggerOffset = -15
let LifebuoySpawnChance = 0.2 // TODO: Compare with the original
let LifeBonusSpawnChance = seq {
    yield! [|0.25; 0.125;|]
    yield! Array.create 7 0.0625
    while true do yield 0.
} // TODO: Compare with the original
