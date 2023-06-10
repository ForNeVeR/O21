namespace O21.Game.U95

open System.IO
open System.Threading.Tasks

open JetBrains.Lifetimes
open Raylib_CsLo

open O21.Game
open O21.Game.Help
open O21.Game.Localization.Translations

type U95Data private(sprites: Sprites,
                     sounds: Map<SoundType, Sound>,
                     midiFilePath: string,
                     help: Language -> DocumentFragment[],
                     levels: Level[]) =

    let mutable helpCache = Map.empty

    member this.Sprites: Sprites = sprites
    member this.Sounds: Map<SoundType, Sound> = sounds
    member this.MidiFilePath = midiFilePath
    member this.Help(language: Language): DocumentFragment[] =
        match helpCache |> Map.tryFind language with
        | Some fragments -> fragments
        | None ->
            let fragments = help language
            helpCache <- helpCache |> Map.add language fragments
            fragments
    member this.Levels = levels
    static member Load (lifetime: Lifetime) (loadController: LoadController) (directory: string): Task<U95Data> =
        task {
            let loadTranslation() = async {
                do! Async.SwitchToThreadPool()
                return Translation(DefaultLanguage)
            }

            // TODO[#101]: Get the translation from local content (should be loaded)
            // TODO[#99]: Get the active language from settings
            let! translation = loadTranslation()

            loadController.ReportProgress(translation.LoadingData, 0.2)
            let! sprites = Sprites.LoadFrom lifetime directory

            loadController.ReportProgress(translation.LoadingData, 0.4)
            let hlpFilePath = Path.Combine(directory, "U95.HLP")
            let help (language: Language) =
                match language.HelpRequestType with
                | HelpRequestType.WinHelpFile -> HlpFile.Load lifetime hlpFilePath
                | HelpRequestType.MarkdownFile ->
                    let markdownFilePath = Path.Combine(Paths.HelpFolder, $"{language.Name}.md")
                    MarkdownHelp.Load lifetime hlpFilePath markdownFilePath

            loadController.ReportProgress(translation.LoadingData, 0.6)
            let! sounds = Sound.Load directory
            let midiFilePath = Path.Combine(directory, "U95.MID")

            loadController.ReportProgress(translation.LoadingData, 0.8)
            let! level = Level.Load directory 1 2

            loadController.ReportProgress(translation.CatchingUp, 1.0)
            return U95Data(sprites, sounds, midiFilePath, help, [| level |])
        }
