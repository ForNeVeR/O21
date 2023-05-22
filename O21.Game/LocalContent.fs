namespace O21.Game

open System
open System.IO
open System.Threading
open Raylib_CsLo

#nowarn "9"

/// The game content that's always available locally, even if the main game resources haven't been downloaded, yet.
type LocalContent = {
    UiFontRegular: Font
    UiFontBold: Font
    LoadingTexture: Texture
} with

    static member Load(): Async<LocalContent> = async {
        let x = SynchronizationContext.Current

        do! Async.SwitchToThreadPool() // TODO: For test yet; implement good asynchronous loading
        let binDir = Path.GetDirectoryName(Environment.ProcessPath)
        let pathToResource fileName =
            Path.Combine(binDir, "Resources", fileName)
        let fontChars = [|
            for i in 0..95 -> 32 + i // Basic ASCII characters
            for i in 0..255 -> 0x400 + i // Cyrillic characters
            yield int '…'
        |]

        let loadFont(path: string) =
            use ptr = fixed fontChars
            Raylib.LoadFontEx(path, 24, ptr, fontChars.Length)

        do! Async.SwitchToContext x
        let regular = loadFont(pathToResource "Inter-Regular.otf")
        let bold = loadFont(pathToResource "Inter-Bold.otf")
        let loading = Raylib.LoadTexture(pathToResource "submarine.png")

        return {
            UiFontRegular = regular
            UiFontBold = bold
            LoadingTexture = loading
        }
    }
