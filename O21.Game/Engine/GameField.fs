namespace O21.Game.Engine

[<Struct>]
type Point =
    | Point of int * int

    static member (+) (Point(x1, y1), Vector(x2, y2)): Point = Point(x1 + x2, y1 + y2)

and [<Struct>] Vector =
    | Vector of int * int

    static member (+) (Vector(x1, y1), Vector(x2, y2)): Vector = Vector(x1 + x2, y1 + y2)
    static member (*) (Vector(x, y), i: int): Vector = Vector(x * i, y * i)
