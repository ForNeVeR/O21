// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

[<Struct>]
type Point =
    | Point of int * int

    static member (+) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 + x2, y1 + y2)
    static member (-) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 - x2, y1 - y2)

    member this.X: int = let (Point(x, _)) = this in x
    member this.Y: int = let (Point(_, y)) = this in y
    
and [<Struct>] Vector =
    | Vector of int * int

    static member (+) (Vector(x1, y1), Vector(x2, y2)): Vector = Vector(x1 + x2, y1 + y2)
    static member (*) (Vector(x1, y1), Vector(x2, y2)): Vector = Vector(x1 * x2, y1 * y2)
    static member (*) (Vector(x, y), i: int): Vector = Vector(x * i, y * i)

    member this.X: int = let (Vector(x, _)) = this in x
    member this.Y: int = let (Vector(_, y)) = this in y
    
    static member Zero: Vector = Vector(0, 0)

[<Struct>]
type Box =
    { TopLeft: Point; Size: Vector }
    
    member this.TopRight: Point = this.TopLeft + Vector(this.Size.X - 1, 0)
    member this.BottomLeft: Point = this.TopLeft + Vector(0, this.Size.Y - 1)
    member this.BottomRight: Point = this.TopLeft + this.Size - Vector(1, 1)

[<Struct>]
type Trigger =
    | VerticalTrigger of X:int
    | HorizontalTrigger of Y:int

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
    static member (*) (direction: ObliqueDirection, catheters: Vector) =
        let x_value = direction.XDirection * catheters.X
        let y_value = direction.YDirection * catheters.Y
        Vector(x_value, y_value)

[<Struct; RequireQualifiedAccess>]
type Collision =
    | None
    /// Means the object is completely out of bounds, i.e. not visible on the game field.
    | OutOfBounds
    /// At least one pixel of the object intersects with a brick.
    | CollidesBrick
    /// Tells, that the object collides other box\boxes in its environment
    | CollidesObject of count: int
