// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game
open O21.Game.Engine.Environments
open O21.Game.Engine.Geometry
open O21.Game.U95

[<RequireQualifiedAccess>]
type EnemyEffect<'e> =
    | Update of 'e
    | PlayerHit of id: int
    | Die
    /// For enemies that leave the screen without being destroyed.
    | Despawn

type Fish = {
    TopLeft: Point
    Type: int
    Velocity: Vector
    Direction: HorizontalDirection
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.FishSizes[this.Type] }

    member this.Tick(fishEnv: EnemyEnv): Fish EnemyEffect =
        let newPosition = this.TopLeft + this.Velocity
        let newFish = { this with TopLeft = newPosition }
        match CheckCollision fishEnv.Level newFish.Box Array.empty with // TODO[#27]: Player and bullet collision
        | Collision.None -> EnemyEffect.Update newFish
        | Collision.OutOfBounds -> EnemyEffect.Despawn
        | Collision.CollidesBrick ->
            // TODO[#27]: Fish behavior: up/down
            // TODO[#27]: Fish behavior: turn
            // TODO[#27]: Fish behavior: randomize speed
            EnemyEffect.Update this
        | Collision.CollidesObject count ->
            // TODO[#27]: Player and bullet collision
            EnemyEffect.Update newFish

    static member Default = {
        TopLeft = Point(0, 0)
        Type = 0
        Velocity = Vector(0, 0)
        Direction = HorizontalDirection.Right
    }

    static member private Random(position, random: ReproducibleRandom) =
        let direction = if random.NextBool() then HorizontalDirection.Left else HorizontalDirection.Right
        {
            TopLeft = position
            Type = random.NextExcluding GameRules.FishKinds
            Velocity = Vector(direction * GameRules.FishBaseVelocity, 0)
            Direction = direction
        }

    static member SpawnOnLevelEntry(
        random: ReproducibleRandom,
        level: Level,
        player: Player
    ): Fish[] =

        let playerExclusiveZones =
            let box = player.Box
            {
                TopLeft = Point(0, box.TopLeft.Y)
                Size = Vector(GameRules.LevelWidth, box.Size.Y)
            }
        let allowedToSpawn(fish: Fish) =
            CheckCollision level fish.Box [| playerExclusiveZones |] = Collision.None

        [|
            // TODO: Pick the actual level size instead of hardcoding here. In the future, we might wish to have levels
            // of different sizes in the same game. This will require the level to be aware of its contents, though.
            for x in 0 .. GameRules.BrickSize.X .. GameRules.LevelWidth - 1 do
                for y in 0 .. GameRules.BrickSize.Y .. GameRules.LevelHeight - 1 do
                    let fish = Fish.Random(Point(x, y), random)
                    if allowedToSpawn fish && random.Chance GameRules.LevelEntryFishSpawnProbability then
                        yield fish
        |]

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
