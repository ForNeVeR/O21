module O21.Game.Engine.GameRules

open System

[<Literal>]
let NormalShotCooldownTicks = 100 // TODO: Compare with the original

[<Literal>]
let LevelWidth = 600

[<Literal>]
let LevelHeight = 400

[<Literal>]
let TicksPerSecond = 10.0

let MaxPlayerVelocity = 3
let ClampVelocity(Vector(x, y)): Vector =
   Vector(
       Math.Clamp(x, -MaxPlayerVelocity, MaxPlayerVelocity),
       Math.Clamp(y, -MaxPlayerVelocity, MaxPlayerVelocity)
   )

[<Literal>]
let BulletVelocity = 5 // TODO: Compare with the original
