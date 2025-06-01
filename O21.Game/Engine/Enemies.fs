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
    | PlayerHit of id: Guid
    | Die
    /// For enemies that leave the screen without being destroyed.
    | Despawn

type Fish = {
    Id: Guid // TODO: Think about some better identifier mechanism.
    TopLeft: Point
    Type: int
    /// The current velocity the fish will be moving in the designated direction.
    AbsoluteVelocity: int
    HorizontalDirection: HorizontalDirection
    VerticalDirection: VerticalDirection
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.FishSizes[this.Type] }

    member this.Tick(fishEnv: EnemyEnv): Fish EnemyEffect =
        let newFish = this.WithNextPosition fishEnv.Level
        match CheckCollision fishEnv.Level newFish.Box Array.empty with // TODO[#27]: Player and bullet collision
        | Collision.None -> EnemyEffect.Update newFish
        | Collision.OutOfBounds -> EnemyEffect.Despawn
        | Collision.CollidesBrick ->
            // TODO[#27]: Fish behavior: up/down
            // TODO[#27]: Fish behavior: turn
            // TODO[#27]: Fish behavior: randomize speed
            EnemyEffect.Update this
        | Collision.CollidesObject _ ->
            // TODO[#27]: Player and bullet collision
            EnemyEffect.Update newFish

    member private this.WithNextPosition level: Fish =
        // TODO: Stick to the wall if there's any space
        let checkState(state: Fish) =
            match CheckCollision level state.Box Array.empty with
            | Collision.CollidesBrick -> None
            | _ -> Some state

        let moveHorizontally keepDirection =
            let newDirection = if keepDirection then this.HorizontalDirection else this.HorizontalDirection.Invert()
            let newState = {
                this with
                    TopLeft = this.TopLeft.Move(newDirection, this.AbsoluteVelocity)
                    HorizontalDirection = newDirection
            }
            checkState newState

        let moveVertically keepDirection =
            let newDirection = if keepDirection then this.VerticalDirection else this.VerticalDirection.Invert()
            let newState = {
                this with
                    TopLeft = this.TopLeft.Move(newDirection, this.AbsoluteVelocity)
                    VerticalDirection = newDirection
            }
            checkState newState

        moveHorizontally true
        |> Option.orElseWith(fun () -> moveVertically true)
        |> Option.orElseWith(fun () -> moveVertically false)
        |> Option.orElseWith(fun () -> moveHorizontally false)
        |> Option.defaultValue this


    static member Default = {
        Id = Guid.Empty
        TopLeft = Point(0, 0)
        Type = 0
        AbsoluteVelocity = GameRules.FishBaseVelocity
        HorizontalDirection = HorizontalDirection.Right
        VerticalDirection = VerticalDirection.Up
    }

    static member private Random(position, random: ReproducibleRandom) =
        let direction = if random.NextBool() then HorizontalDirection.Left else HorizontalDirection.Right
        {
            Fish.Default with
                Id = Guid.NewGuid()
                TopLeft = position
                Type = random.NextExcluding GameRules.FishKinds
                AbsoluteVelocity = GameRules.FishBaseVelocity
                HorizontalDirection = direction
        }

    static member SpawnNew(topLeft: Point, ``type``: int, velocity: int, direction: HorizontalDirection): Fish = {
        Fish.Default with
            Id = Guid.NewGuid()
            TopLeft = topLeft
            Type = ``type``
            AbsoluteVelocity = velocity
            HorizontalDirection = direction
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
    Id: Guid // TODO: Think about some better identifier mechanism.
    Type: int
    TopLeft: Point
    State: BombState
} with
    static member Create (random: ReproducibleRandom) (position: Point)=
        {
            Id = Guid.NewGuid()
            Type = random.NextExcluding GameRules.BombKinds
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
