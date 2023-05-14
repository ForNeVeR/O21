namespace O21.Game

open System
open System.IO
open Raylib_CsLo

#nowarn "9"

type GameContent = {
    UiFontRegular: Font
    UiFontBold: Font
    LoadingTexture: Texture
} with
    static member Load(): GameContent = 
        let binDir = Path.GetDirectoryName(Environment.ProcessPath)
        let pathToResource fileName =
            Path.Combine(binDir, "Resources", fileName)
        let fontChars = [|
            for i in 0..95 -> 32 + i // Basic ASCII characters
            for i in 0..255 -> 0x400 + i // Cyrillic characters
            yield int '…'
        |]
        use ptr = fixed fontChars
        {
            UiFontRegular = Raylib.LoadFontEx(pathToResource "Inter-Regular.otf", 12, ptr, fontChars.Length)
            UiFontBold = Raylib.LoadFontEx(pathToResource "Inter-Bold.otf", 12, ptr, fontChars.Length)
            LoadingTexture = Raylib.LoadTexture(pathToResource "submarine.png")
        }
