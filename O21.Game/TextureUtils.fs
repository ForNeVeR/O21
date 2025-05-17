// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.TextureUtils

open JetBrains.Lifetimes
open Microsoft.FSharp.NativeInterop
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Images
open type Raylib_CSharp.Raylib
open Oddities.Resources
open Raylib_CSharp.Textures

let private transparentColor = struct(0xFFuy, 0xFFuy, 0xFFuy)
let private isColor(struct(r1, g1, b1), struct(r2, g2, b2)) =
    r1 = r2 && g1 = g2 && b1 = b2

#nowarn "9"

let CreateTransparentSprite (lifetime: Lifetime) (colors: Dib) (transparency: Dib): Texture2D =
    let width = colors.Width
    let height = colors.Height
    let colors = Array.init (width * height) (fun i ->
        let x = i % width
        let y = i / width
        let isTransparent = isColor(transparency.GetPixel(x, y), transparentColor)
        if isTransparent then
            Color.Blank
        else
            let struct(r, g, b) = colors.GetPixel(x, y)
            Color(r, g, b, 255uy)
    )
    use colorsPtr = fixed colors
    let image = Image(
        Data = NativePtr.toNativeInt colorsPtr,
        Width = width,
        Height = height,
        Format = PixelFormat.UncompressedR8G8B8A8,
        Mipmaps = 1
    )
    RaylibUtils.LoadTextureFromImage lifetime image

let CreateSprite (lifetime: Lifetime) (colors: Dib): Texture2D =
    let width = colors.Width
    let height = colors.Height
    let colors = Array.init (width * height) (fun i ->
        let x = i % width
        let y = i / width
        let struct(r, g, b) = colors.GetPixel(x, y)
        Color(r, g, b, 255uy)
    )
    use colorsPtr = fixed colors
    let image = Image(
        Data = NativePtr.toNativeInt colorsPtr,
        Width = width,
        Height = height,
        Format = PixelFormat.UncompressedR8G8B8A8,
        Mipmaps = 1
    )
    RaylibUtils.LoadTextureFromImage lifetime image
