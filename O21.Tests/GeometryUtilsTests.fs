module GeometryUtilsTests

open System.Numerics
open Xunit

open O21.Game.GeometryUtils

[<Fact>]
let ``GenerateSquareSector 000%`` () =
    Assert.Equal<Vector2>(Array.empty, GenerateSquareSector 0.0)
    Assert.Equal<Vector2>(Array.empty, GenerateSquareSector -0.1)

[<Fact>]
let ``GenerateSquareSector 012.5%`` () =
    Assert.Equal<Vector2>([|
        Vector2(0.5f, 0.5f)
        Vector2(0.5f, 1f)
        Vector2(1f, 1f)
        Vector2(0.5f, 0.5f)
    |], GenerateSquareSector 0.125)

[<Fact>]
let ``GenerateSquareSector 025%`` () =
    Assert.Equal<Vector2>([|
        Vector2(0.5f, 0.5f)
        Vector2(0.5f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0.5f)
        Vector2(0.5f, 0.5f)
    |], GenerateSquareSector 0.25)

[<Fact>]
let ``GenerateSquareSector 037.5%`` () =
    Assert.Equal<Vector2>([|
        Vector2(0.5f, 0.5f)
        Vector2(0.5f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0.5f)
    |], GenerateSquareSector 0.375)

[<Fact>]
let ``GenerateSquareSector 050%`` () =
    Assert.Equal<Vector2>([|
        Vector2(0.5f, 0.5f)
        Vector2(0.5f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0.5f, 0f)
        Vector2(0.5f, 0.5f)
    |], GenerateSquareSector 0.5)

[<Fact>]
let ``GenerateSquareSector 100%`` () =
    let fullSquare = [|
        Vector2(0f, 0f)
        Vector2(0f, 1f)
        Vector2(1f, 1f)
        Vector2(1f, 0f)
        Vector2(0f, 0f)
    |]
    Assert.Equal<Vector2>(fullSquare, GenerateSquareSector 1.0)
    Assert.Equal<Vector2>(fullSquare, GenerateSquareSector 2.0)
