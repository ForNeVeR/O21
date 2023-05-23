namespace O21.Game

open System
open System.IO

open Raylib_CsLo

/// The game content that's always available locally, even if the main game resources haven't been downloaded, yet.
type LocalContent = {
    UiFontRegular: Font
    UiFontBold: Font
    LoadingTexture: Texture
} with
    static member Load(): Async<LocalContent> = async {
        let binDir = Path.GetDirectoryName(Environment.ProcessPath)
        let pathToResource fileName =
            Path.Combine(binDir, "Resources", fileName)
        let fontChars = [|
            for i in 0..95 -> 32 + i // Basic ASCII characters
            for i in 0..255 -> 0x400 + i // Cyrillic characters
            yield int '…'
        |]
        let fontSize = 24

        let loadFont path = async {
            let! ct = Async.CancellationToken
            let! data = Async.AwaitTask <| File.ReadAllBytesAsync(path, ct)
            return RaylibUtils.LoadFontFromMemory (Path.GetExtension path) data fontSize fontChars
        }

        let loadTexture path = async {
            let! ct = Async.CancellationToken
            let! data = Async.AwaitTask <| File.ReadAllBytesAsync(path, ct)
            return RaylibUtils.LoadTextureFromMemory (Path.GetExtension path) data
        }

        let! regular = loadFont <| pathToResource "Inter-Regular.otf"
        let! bold = loadFont <| pathToResource "Inter-Bold.otf"
        let! loading = loadTexture <| pathToResource "submarine.png"

        return {
            UiFontRegular = regular
            UiFontBold = bold
            LoadingTexture = loading
        }
    }
