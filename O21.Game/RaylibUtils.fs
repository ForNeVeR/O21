// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.RaylibUtils

open System
open System.Numerics
open JetBrains.Lifetimes
open Microsoft.FSharp.NativeInterop
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Fonts
open Raylib_CSharp.IO
open Raylib_CSharp.Images
open Raylib_CSharp.Rendering
open Raylib_CSharp.Rendering.Gl
open Raylib_CSharp.Textures

#nowarn 9

let LoadFontFromMemory (lifetime: Lifetime)
                       (fileType: string)
                       (data: byte[])
                       (fontSize: int)
                       (fontChars: int[]): Font =
    lifetime.Bracket<Font>(
        Func<Font>(fun () ->
            Font.LoadFromMemory(
                fileType,
                ReadOnlySpan(data),
                fontSize,
                ReadOnlySpan(fontChars)
            )),
        Action<Font>(_.Unload())
    ) 

let LoadTextureFromImage (lifetime: Lifetime) (image: Image): Texture2D =
    lifetime.Bracket(
        (fun () -> Texture2D.LoadFromImage image),
        image.Unload
    )

let LoadTextureFromMemory (lifetime: Lifetime) (fileType: string) (data: byte[]): Texture2D =
    let dataSpan = Span(data)

    let image = Image.LoadFromMemory(fileType, dataSpan)
    if NativePtr.ofNativeInt<byte> image.Data = NativePtr.nullPtr // Huh? https://github.com/dotnet/fsharp/issues/15254
    then failwith $"Cannot load image of type {fileType} from {data.Length} bytes of data."

    try
        LoadTextureFromImage lifetime image
    finally
        image.Unload()

let DrawTexturePoly (texture: Texture2D) (center: Vector2) (pointCoords: Vector2[]) (texCoords: Vector2[]) (tint: Color) : unit =
    let pointCount = pointCoords.Length
    if pointCount <> texCoords.Length then
        failwith "point count != texcoord count"
    if pointCount = 0 then
        ()

    Graphics.BeginBlendMode(BlendMode.AlphaPremultiply)
    RlGl.Begin(DrawMode.Triangles)
    
    RlGl.SetTexture(texture.Id)
    RlGl.Color4ub(tint.R, tint.G, tint.B, tint.A)
    
    for i in 0..pointCount-2 do
        RlGl.TexCoord2F(0.5f, 0.5f)
        RlGl.Vertex2F(center.X, center.Y)

        RlGl.TexCoord2F(texCoords[i].X, texCoords[i].Y)
        RlGl.Vertex2F(pointCoords[i].X + center.X, pointCoords[i].Y + center.Y)
        
        RlGl.TexCoord2F(texCoords[i + 1].X, texCoords[i + 1].Y)
        RlGl.Vertex2F(pointCoords[i + 1].X + center.X, pointCoords[i + 1].Y + center.Y)
        
    RlGl.End()
    RlGl.SetTexture(0u)
    
    Graphics.EndBlendMode()
