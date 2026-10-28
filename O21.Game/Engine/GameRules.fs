// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Engine.GameRules

open System
open O21.Game
open O21.Game.U95

// Values are verified against the original U95.EXE (Win16, Borland C++ 1991).

// ----------------------- Screen Constants -----------------------

[<Literal>]
let GameScreenWidth = 600

[<Literal>]
let GameScreenHeight = 390

// ----------------------- Player Constants -----------------------

[<Literal>]
let ShotCooldownTicks = 15

[<Literal>]
let ShotCooldownTicksWithZeroCooldownAbility = 3

[<Literal>]
let MaxPlayerLives = 9

[<Literal>]
let InitialPlayerLives = 5

/// Original clamps the horizontal velocity asymmetrically: -4..+5.
[<Literal>]
let MaxPlayerVelocityRight = 5

[<Literal>]
let MaxPlayerVelocityLeft = 4

[<Literal>]
let MaxPlayerVerticalVelocity = 4

/// The largest absolute player velocity (used by the movement animation).
[<Literal>]
let MaxPlayerVelocity = MaxPlayerVelocityRight

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
let PostDeathFreezeTicks = 3

[<Literal>]
let OxygenUnitPeriod = 20

[<Literal>]
let BulletLifetime = 10

[<Literal>]
let ExplosiveBulletLifetime = 10

[<Literal>]
let BulletFromExplosiveLifetime = 40

[<Literal>]
let TripleShotDownBulletLifetime = 40

[<Literal>]
let TripleShotUpBulletLifetime = 30

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
let SubtractPointsForShotByExplosiveBullet = 5

// ----------------------- Physics Constants -----------------------

[<Literal>]
let BulletVelocity = 6

[<Literal>]
let ExplosiveBulletVelocity = 8

[<Literal>]
let BulletVelocityInCirclePattern = 5

let BulletVerticalSpread = Vector(0, 3)

/// Upward speed of a triggered bomb; the original picks a random value of 4..7. (rand() % 4 + 4)
let BombVelocityRange = [| 4..7 |]

[<Literal>]
let ParticleSpeed = 3

let ParticlesPeriodRange = [| 6..10 |]

let ParticlesOffsetRange = [| 0; 3 |]

// ----------------------- Object Sizes -----------------------

let BrickSize = Vector(12, 12)
let PlayerSize = Vector(46, 27)
let BulletSize = Vector(6, 6)
let ParticleSize = Vector(5, 5)
let FishSizes = [|Vector(31, 17); Vector(29, 17); Vector(34, 20); Vector(18, 27); Vector(46, 23)|]
let BombSize = Vector(24, 24)
let StaticBonusSize = Vector(36, 36)
let LifebuoySize = Vector(32, 32)
let LifeBonusSize = Vector(36, 21)

// ------------------------------ Enemy Constants ------------------------------
let FishKinds: int = FishSizes.Length
[<Literal>]
let LevelEntryFishSpawnProbability: float = 0.00363 // 0.363%
let FromEdgesFishSpawnProbability (column: int) : float =
    let maxCol = 59
    if column <= maxCol then 1. / float(60 - column) else 0.
let FishHorizontalVelocity = [| 3..5 |]
let FishVerticalVelocity = 5
let BombKinds: int = 5

// ----------------------- Functions -----------------------

/// Relative position of the bullet sprite's top left corner to the player sprite's top corner (from the shooting side)
/// when the bullet appears.
let NewBulletPosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(-1 - BulletSize.X, 14)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(1, 14)

let NewParticlePosition(playerTopForwardCorner: Point, playerDirection: HorizontalDirection) =
    match playerDirection with
    | HorizontalDirection.Left -> playerTopForwardCorner + Vector(17, -ParticleSize.Y)
    | HorizontalDirection.Right -> playerTopForwardCorner + Vector(17 - PlayerSize.X, -ParticleSize.Y)

let ClampVelocity(Vector(x, y)): Vector =
   Vector(
       Math.Clamp(x, -MaxPlayerVelocityLeft, MaxPlayerVelocityRight),
       Math.Clamp(y, -MaxPlayerVerticalVelocity, MaxPlayerVerticalVelocity)
   )

let GetLevelPosition level (coordinates: int * int)  =
            Point(LevelWidth / level.LevelMap[0].Length * fst coordinates,
                  LevelHeight / level.LevelMap.Length * snd coordinates)

// ----------------------- Other Game Constants -----------------------

let StartingLevel = LevelCoordinates(1, 1)
let PlayerStartingPosition = Point(200, 140)
let LevelSizeInTiles = Vector(50, 25)
let BombTriggerOffset = -15 //[| -19 .. -1 |]

/// Original: a lifebuoy (random ability) is placed on every entry to a level to the east of the previous one.
let LifebuoySpawnProbability = 1.0

let LifeBonusSpawnProbability = seq {
    yield 0.
    yield! [|0.25; 0.125;|]
    yield! Array.create 7 0.0625
    while true do yield 0.
}

let StaticBonusKinds = 10
