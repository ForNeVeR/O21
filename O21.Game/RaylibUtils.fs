// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.RaylibUtils

open System
open JetBrains.Lifetimes
open Microsoft.FSharp.NativeInterop
open Raylib_CSharp
open Raylib_CSharp.Fonts
open Raylib_CSharp.IO
open Raylib_CSharp.Images
open Raylib_CSharp.Textures

#nowarn 9

let LoadFontFromMemory (lifetime: Lifetime)
                       (fileType: string)
                       (data: byte[])
                       (fontSize: int)
                       (fontChars: int[]): Font =
    let dataSpan = Span(data)
    let fontCharsPtr = Span(fontChars)
    let font = Font.LoadFromMemory(
        fileType,
        dataSpan,
        fontSize,
        fontCharsPtr
    )
    lifetime.Bracket(
        (fun () ->
            font
        ),
        font.Unload
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
