// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.GeometryUtils

open System
open System.Numerics

let private halfPi = float32 <| Math.PI/2.0

let private IntersectHorizontalLine (angle: float32) : float32 = (0.5f / tan angle) + 0.5f 

let private IntersectWithVerticalLine (angle: float32) : float32 = (0.5f * tan angle) + 0.5f

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
            Vector2(0.5f, 0.5f)
            if p <= 0.125 then Vector2(IntersectHorizontalLine (halfPi - angle), 0f) else
                if p <= 0.375 then Vector2(1f, IntersectWithVerticalLine (angle - halfPi)) else
                    if p <= 0.625 then Vector2(IntersectHorizontalLine (angle - halfPi), 1f) else
                        if p < 0.875 then Vector2(0f, IntersectWithVerticalLine (halfPi - angle)) else
                            Vector2(IntersectHorizontalLine (halfPi - angle), 0f)
                            Vector2(0f, 0f)
                        Vector2(0f, 1f)
                    Vector2(1f, 1f)
                Vector2(1f, 0f)

            Vector2(0.5f, 0f)
            Vector2(0.5f, 0.5f)
        |]
