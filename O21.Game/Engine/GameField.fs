// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

[<Struct>]
type Point =
    | Point of int * int

    static member (+) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 + x2, y1 + y2)

    member this.X: int = let (Point(x, _)) = this in x
    member this.Y: int = let (Point(_, y)) = this in y
    
and [<Struct>] Vector =
    | Vector of int * int

    static member (+) (Vector(x1, y1), Vector(x2, y2)): Vector = Vector(x1 + x2, y1 + y2)
    static member (*) (Vector(x, y), i: int): Vector = Vector(x * i, y * i)

    member this.X: int = let (Vector(x, _)) = this in x
    member this.Y: int = let (Vector(_, y)) = this in y

[<Struct>]
type Box =
    { TopLeft: Point; Size: Vector }
    
    member this.TopRight: Point = this.TopLeft + Vector(this.Size.X, 0)
    member this.BottomLeft: Point = this.TopLeft + Vector(0, this.Size.Y)
    member this.BottomRight: Point = this.TopLeft + this.Size

[<Struct>]
type HorizontalDirection =
    | Left
    | Right
    static member (*) (direction: HorizontalDirection, value: int) =
        match direction with
        | Left -> -value
        | Right -> value

[<Struct>]
type VerticalDirection =
    | Up
    | Down
    static member (*) (direction: VerticalDirection, value: int) =
        match direction with
        | Up -> -value
        | Down -> value
    
[<Struct>]  
type ObliqueDirection = {
    XDirection: HorizontalDirection
    YDirection: VerticalDirection
} with
    static member (*) (direction: ObliqueDirection, xy_catheters: int * int) =
        let x_value = direction.XDirection * fst xy_catheters
        let y_value = direction.YDirection * snd xy_catheters
        Point(x_value, y_value)

[<Struct; RequireQualifiedAccess>]
type Collision =
    | None
    | OutOfBounds
    | TouchesBrick
