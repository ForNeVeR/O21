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
        [|
            yield Vector2(0.5f, 0.5f)
            yield Vector2(0.5f, 1f)
            if p <= 0.125 then yield Vector2(sin angle / 2f * sqrt 2f + 0.5f, 1f)
            else
                yield Vector2(1f, 1f)
                if p <= 0.375 then yield Vector2(1f, cos angle / 2f * sqrt 2f + 0.5f)
                else
                    yield Vector2(1f, 0f)
                    if p <= 0.625 then yield Vector2(sin angle / 2f * sqrt 2f + 0.5f, 0f)
                    else
                        yield Vector2(0f, 0f)
                        if p < 0.875 then yield Vector2(0f, cos angle / 2f * sqrt 2f + 0.5f)
                        else
                            yield Vector2(0f, 1f)
                            yield Vector2(sin angle / 2f * sqrt 2f + 0.5f, 1f)

            yield Vector2(0.5f, 0.5f)
        |]
