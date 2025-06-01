// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open O21.Game

[<Struct>]
type Point =
    | Point of int * int

    static member (+) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 + x2, y1 + y2)
    static member (-) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 - x2, y1 - y2)
    
    static member (%) (Point(x1, y1), Point(x2, y2)): Point =
        let modulo a b = ((a % b) + b) % b
        Point(modulo x1 x2, modulo y1 y2)

    member this.X: int = let (Point(x, _)) = this in x
    member this.Y: int = let (Point(_, y)) = this in y

    member this.Move(direction: HorizontalDirection, distance: int): Point =
        Point(this.X + direction * distance, this.Y)

    member this.Move(direction: VerticalDirection, distance: int): Point =
        Point(this.X, this.Y + direction * distance)

    member this.Up(distance: int): Point = this.Move(VerticalDirection.Up, distance)
    member this.Down(distance: int): Point = this.Move(VerticalDirection.Down, distance)

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
