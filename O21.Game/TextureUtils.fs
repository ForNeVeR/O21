module O21.Game.TextureUtils

open Microsoft.FSharp.NativeInterop
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Resources

let private transparentColor = struct(0xFFuy, 0xFFuy, 0xFFuy)
let private isColor(struct(r1, g1, b1), struct(r2, g2, b2)) =
    r1 = r2 && g1 = g2 && b1 = b2

#nowarn "9"

let CreateTransparentSprite (colors: Dib) (transparency: Dib): Texture =
    let width = colors.Width
    let height = colors.Height
    let colors = Array.init (width * height) (fun i ->
        let x = i % width
        let y = i / width
        let isTransparent = isColor(transparency.GetPixel(x, y), transparentColor)
        if isTransparent then
            BLANK
        else
            let struct(r, g, b) = colors.GetPixel(x, y)
            Color(r, g, b, 255uy)
    )
    use colorsPtr = fixed colors
    let image = Image(
        data = NativePtr.toVoidPtr colorsPtr,
        width = width,
        height = height,
        format = int PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        mipmaps = 1
    )
    let texture = LoadTextureFromImage(image)
    texture

let CreateSprite(colors: Dib): Texture =
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
        data = NativePtr.toVoidPtr colorsPtr,
        width = width,
        height = height,
        format = int PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
        mipmaps = 1
    )
    let texture = LoadTextureFromImage(image)
    texture
