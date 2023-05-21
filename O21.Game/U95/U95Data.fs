namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Raylib_CsLo

open O21.Game.Help
open O21.Game.Localization
open O21.Localization.Translations

type U95Data private (sprites: Sprites, sounds: Map<SoundType, Sound>, help: Language -> DocumentFragment[], levels: Level[]) =

    let mutable helpCache = Map.empty

    member this.Sprites: Sprites = sprites
    member this.Sounds: Map<SoundType, Sound> = sounds
    member this.Help(language: Language): DocumentFragment[] =
        match helpCache |> Map.tryFind language with
        | Some fragments -> fragments
        | None ->
            let fragments = help language
            helpCache <- helpCache |> Map.add language fragments
            fragments
    member this.Levels = levels
    static member Load (directory: string): Task<U95Data> = task {
        let! sprites = Sprites.LoadFrom directory

        let hlpFilePath = Path.Combine(directory, "U95.HLP")
        let help (language: Language) =
            match language.HelpRequestType with
            | HelpRequestType.WinHelpFile -> HlpFile.Load hlpFilePath
            | HelpRequestType.MarkdownFile ->
                let markdownFilePath = Path.Combine(LocalizationPaths.HelpFolder, $"{language.Name}.md")
                MarkdownHelp.Load hlpFilePath markdownFilePath

        let! sounds = Sound.Load directory
        let! level = Level.Load directory 1 2
        return new U95Data(sprites, sounds, help, [| level |])
    }

    interface IDisposable with
        member this.Dispose() =
            (this.Sprites :> IDisposable).Dispose()
            helpCache
            |> Map.iter(fun _ fragments -> fragments |> Array.iter(fun f -> (f :> IDisposable).Dispose()))
