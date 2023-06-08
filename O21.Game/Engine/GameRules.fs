module O21.Game.Engine.GameRules

open System

[<Literal>]
let ShotCooldownSec = 1.0

[<Literal>]
let GameWidth = 600

[<Literal>]
let GameHeight = 400

[<Literal>]
let TicksPerSecond = 10.0

let MaxPlayerVelocity = 3
let ClampVelocity(Vector(x, y)): Vector =
   Vector(
       Math.Clamp(x, -MaxPlayerVelocity, MaxPlayerVelocity),
       Math.Clamp(y, -MaxPlayerVelocity, MaxPlayerVelocity)
   )
