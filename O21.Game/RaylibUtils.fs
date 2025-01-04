// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.RaylibUtils

open JetBrains.Lifetimes
open Microsoft.FSharp.NativeInterop
open Raylib_CsLo

#nowarn 9

let LoadFontFromMemory (lifetime: Lifetime)
                       (fileType: string)
                       (data: byte[])
                       (fontSize: int)
                       (fontChars: int[]): Font =
    use dataPtr = fixed data
    use fontCharsPtr = fixed fontChars
    lifetime.Bracket(
        (fun () ->
            Raylib.LoadFontFromMemory(
                fileType,
                dataPtr,
                data.Length,
                fontSize,
                fontCharsPtr,
                fontChars.Length
            )
        ),
        Raylib.UnloadFont
    )

let LoadTextureFromImage (lifetime: Lifetime) (image: Image): Texture =
    lifetime.Bracket(
        (fun () -> Raylib.LoadTextureFromImage image),
        Raylib.UnloadTexture
    )

let LoadTextureFromMemory (lifetime: Lifetime) (fileType: string) (data: byte[]): Texture =
    use dataPtr = fixed data

    let image = Raylib.LoadImageFromMemory(fileType, dataPtr, data.Length)
    if NativePtr.ofVoidPtr<byte> image.data = NativePtr.nullPtr // Huh? https://github.com/dotnet/fsharp/issues/15254
    then failwith $"Cannot load image of type {fileType} from {data.Length} bytes of data."

    try
        LoadTextureFromImage lifetime image
    finally
        Raylib.UnloadImage image
