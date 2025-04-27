// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT
module O21.Game.Engine.Environments

open O21.Game.U95

type PlayerEnv = {
    Level: Level
    BulletColliders: Box[]
    BombColliders: Box[]
    FishColliders: Box[]
    StaticBonusColliders: Box[]
    LifebuoyCollider: Box Option
    LifeBonusCollider: Box Option
} with
    member this.BonusColliders = seq {
        yield! this.StaticBonusColliders
        yield! Option.toArray this.LifebuoyCollider
        yield! Option.toArray this.LifeBonusCollider
    }
    
    member this.EnemyColliders = seq {
        yield! this.BulletColliders
        yield! this.FishColliders
    }

type EnemyEnv = {
    Level: Level
    PlayerCollider: Box
    BulletColliders: Box[]
}

type BonusEnv = EnemyEnv
