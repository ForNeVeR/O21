namespace O21.Game

open System
open System.IO
open Raylib_CsLo

#nowarn "9"

type Content = {
    UiFontRegular: Font
    UiFontBold: Font
} with

    static member Load() =
        let workingDir = Path.GetDirectoryName(Environment.ProcessPath)

        let pathToResource fileName =
            Path.Combine(workingDir, "Resources", fileName)

        let fontChars = [|
            for i in 0..95 -> 32 + i // Basic ASCII characters
            for i in 0..255 -> 0x400 + i // Cyrillic characters
        |]

        use ptr = fixed fontChars

        {
            UiFontRegular = Raylib.LoadFontEx(pathToResource "Inter-Regular.otf", 24, ptr, fontChars.Length)
            UiFontBold = Raylib.LoadFontEx(pathToResource "Inter-Bold.otf", 24, ptr, fontChars.Length)
        }
