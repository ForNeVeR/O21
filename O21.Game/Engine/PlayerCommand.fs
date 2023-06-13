namespace O21.Game.Engine

[<Struct>]
type PlayerCommand =
    | VelocityDelta of Vector
    | Shoot
