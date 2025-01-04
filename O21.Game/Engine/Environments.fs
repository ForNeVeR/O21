module O21.Game.Engine.Environments

open O21.Game.U95

type PlayerEnv = {
    Level: Level
    BulletColliders: Box[]
    EnemyColliders: Box[]
    BonusColliders: Box[]
}

type EnemyEnv = {
    Level: Level
    PlayerCollider: Box
    BulletColliders: Box[]
}

type BonusEnv = EnemyEnv
