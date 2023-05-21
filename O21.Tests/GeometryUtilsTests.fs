module GeometryUtilsTests

open System.Globalization
open System.Numerics
open Xunit

open O21.Game.GeometryUtils

let assertPolygonEqual (a: Vector2[]) (b: Vector2[]) =
    let maxError = 0.001f
    let fail() =
        let stringifyD (d: float32) = d.ToString("F3", CultureInfo.InvariantCulture)
        let stringifyV = String.concat ", " << Seq.map (fun (v: Vector2) -> $"({stringifyD v.X}, {stringifyD v.Y})")
        Assert.Fail $"Should be equal:\n{stringifyV a}\n{stringifyV b}"

    if a.Length <> b.Length then fail()
    for a, b in Seq.zip a b do
        if (abs(a.X - b.X) > maxError || abs(a.Y - b.Y) > maxError) then fail()

[<Fact>]
let ``GenerateSquareSector 000%`` () =
    assertPolygonEqual Array.empty <| GenerateSquareSector 0.0
    assertPolygonEqual Array.empty <| GenerateSquareSector -0.1

[<Fact>]
let ``GenerateSquareSector 012.5%`` () =
    assertPolygonEqual [|
        Vector2(0.5f, 0.5f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0f)
        Vector2(0.5f, 0.5f)
    |] <| GenerateSquareSector 0.125

[<Fact>]
let ``GenerateSquareSector 025%`` () =
    assertPolygonEqual [|
        Vector2(0.5f, 0.5f)
        Vector2(1f, 0.5f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0f)
        Vector2(0.5f, 0.5f)
    |] <| GenerateSquareSector 0.25

[<Fact>]
let ``GenerateSquareSector 037.5%`` () =
    assertPolygonEqual [|
        Vector2(0.5f, 0.5f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0f)
        Vector2(0.5f, 0.5f)
    |] <| GenerateSquareSector 0.375

[<Fact>]
let ``GenerateSquareSector 050%`` () =
    assertPolygonEqual [|
        Vector2(0.5f, 0.5f)
        Vector2(0.5f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0f)
        Vector2(0.5f, 0.5f)
    |] <| GenerateSquareSector 0.5

[<Fact>]
let ``GenerateSquareSector 100%`` () =
    let fullSquare = [|
        Vector2(0f, 0f)
        Vector2(0f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0f, 0f)
    |]
    assertPolygonEqual fullSquare <| GenerateSquareSector 1.0
    assertPolygonEqual fullSquare <| GenerateSquareSector 2.0
