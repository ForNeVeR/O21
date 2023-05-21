module O21.Game.GeometryUtils

open System
open System.Numerics

let GenerateSquareSector: double -> Vector2[] = function
    | x when x <= 0.0 -> Array.empty
    | x when x >= 1.0 -> [|
            Vector2(0f, 0f)
            Vector2(0f, 1f)
            Vector2(1f, 1f)
            Vector2(1f, 0f)
            Vector2(0f, 0f)
        |]
    | p ->
        let angle = float32 <| 2.0 * Math.PI * p
        let x = sin angle / 2f * sqrt 2f + 0.5f
        let y = -cos angle / 2f * sqrt 2f + 0.5f
        [|
            Vector2(0.5f, 0.5f)
            if p <= 0.125 then Vector2(x, 0f) else
                if p <= 0.375 then Vector2(1f, y) else
                    if p <= 0.625 then Vector2(x, 1f) else
                        if p < 0.875 then Vector2(0f, y) else
                            Vector2(x, 0f)
                            Vector2(0f, 0f)
                        Vector2(0f, 1f)
                    Vector2(1f, 1f)
                Vector2(1f, 0f)

            Vector2(0.5f, 0f)
            Vector2(0.5f, 0.5f)
        |]
