namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Raylib_CsLo

open O21.Game
open O21.Game.Help
open O21.Game.Localization.Translations

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
    static member Load (loadController: LoadController) (directory: string): Task<U95Data> = task {
        let loadTranslation() = async {
            do! Async.SwitchToThreadPool()
            return Translation(DefaultLanguage)
        }

        // TODO[#101]: Get the translation from local content (should be loaded)
        // TODO[#99]: Get the active language from settings
        let! translation = loadTranslation()

        loadController.ReportProgress(translation.LoadingData, 0.2)
        let! sprites = Sprites.LoadFrom directory

        loadController.ReportProgress(translation.LoadingData, 0.4)
        let hlpFilePath = Path.Combine(directory, "U95.HLP")
        let help (language: Language) =
            match language.HelpRequestType with
            | HelpRequestType.WinHelpFile -> HlpFile.Load hlpFilePath
            | HelpRequestType.MarkdownFile ->
                let markdownFilePath = Path.Combine(Paths.HelpFolder, $"{language.Name}.md")
                MarkdownHelp.Load hlpFilePath markdownFilePath

        loadController.ReportProgress(translation.LoadingData, 0.6)
        let! sounds = Sound.Load directory

        loadController.ReportProgress(translation.LoadingData, 0.8)
        let! level = Level.Load directory 1 2

        loadController.ReportProgress(translation.CatchingUp, 1.0)
        return new U95Data(sprites, sounds, help, [| level |])
    }

    interface IDisposable with
        member this.Dispose() =
            (this.Sprites :> IDisposable).Dispose()
            helpCache
            |> Map.iter(fun _ fragments -> fragments |> Array.iter(fun f -> (f :> IDisposable).Dispose()))
