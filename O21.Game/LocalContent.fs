namespace O21.Game

open System
open System.IO
open Raylib_CsLo

#nowarn "9"

/// The game content that's always available locally, even if the main game resources haven't been downloaded, yet.
type LocalContent = {
    UiFontRegular: Font
    UiFontBold: Font
    LoadingTexture: Texture
} with

    static member Load() =
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
            UiFontRegular = Raylib.LoadFontEx(pathToResource "Inter-Regular.otf", 24, ptr, fontChars.Length)
            UiFontBold = Raylib.LoadFontEx(pathToResource "Inter-Bold.otf", 24, ptr, fontChars.Length)
            LoadingTexture = Raylib.LoadTexture(pathToResource "submarine.png")
        }
