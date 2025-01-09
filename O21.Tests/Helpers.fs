module O21.Tests.Helpers

open O21.Game.U95
open O21.Game.Engine.Environments

let getEmptyPlayerEnvWithLevel (level: Level) =
    {
        Level = level
        BulletColliders = [||]
        EnemyColliders = [||]
        BonusColliders = [||]
    }
