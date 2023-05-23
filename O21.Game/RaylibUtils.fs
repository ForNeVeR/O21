module O21.Game.RaylibUtils

open Microsoft.FSharp.NativeInterop
open Raylib_CsLo

#nowarn "9"

let LoadFontFromMemory (fileType: string) (data: byte[]) (fontSize: int) (fontChars: int[]): Font =
    use dataPtr = fixed data
    use fontCharsPtr = fixed fontChars
    Raylib.LoadFontFromMemory(fileType, dataPtr, data.Length, fontSize, fontCharsPtr, fontChars.Length)

let LoadTextureFromMemory (fileType: string) (data: byte[]): Texture =
    use dataPtr = fixed data

    let image = Raylib.LoadImageFromMemory(fileType, dataPtr, data.Length)
    if NativePtr.ofVoidPtr<byte> image.data = NativePtr.nullPtr // Huh? https://github.com/dotnet/fsharp/issues/15254
    then failwith $"Cannot load image of type {fileType} from {data.Length} bytes of data."

    try
        Raylib.LoadTextureFromImage image
    finally
        Raylib.UnloadImage image
