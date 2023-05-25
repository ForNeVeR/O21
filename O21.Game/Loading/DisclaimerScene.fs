namespace O21.Game.Loading

open System.IO
open System.Numerics
open System.Threading.Tasks

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Localization
open O21.Game.Scenes

type DisclaimerScene(config: Config) =
    let onDiskChecker = task {
        if Directory.Exists config.U95DataDirectory then
            let hashFilePath = Paths.U95ContentHashFile config.U95DataDirectory
            if File.Exists hashFilePath then
                let! actualHash = File.ReadAllTextAsync hashFilePath
                let! contentConfig = Downloader.LoadContentConfiguration()
                return actualHash.Trim() = contentConfig.Sha256
            else return false
        else return false
    }

    let mutable areFilesOnDisk = None

    // TODO: Localization controls on this page
    let language = Translations.DefaultLanguage
    let disclaimerAccepted = TaskCompletionSource<unit>()

    let mutable content = Unchecked.defaultof<_>
    let mutable disclaimerHeight = 0f
    let fontSizePx = 24f
    let mutable acceptButton = Unchecked.defaultof<Button>
    let mutable rejectButton = Unchecked.defaultof<Button>

    let disclaimerText lang = (Translations.Translation lang).LegalDisclaimerText
    let drawDisclaimer lang =
        DrawTextEx(
            content.UiFontRegular,
            disclaimerText lang,
            Vector2(0f, 0f),
            fontSizePx,
            0f,
            WHITE
        )

    interface ILoadingScene<LocalContent, unit> with
        member _.Init newContent =
            content <- newContent
            disclaimerHeight <- MeasureTextEx(
                content.UiFontRegular,
                disclaimerText language,
                float32 fontSizePx,
                0f
            ).Y
            acceptButton <- Button.Create(
                content.UiFontRegular,
                (fun l -> (Translations.Translation l).Accept),
                Vector2(0f, disclaimerHeight + 10f),
                language
            )
            rejectButton <- Button.Create(
                content.UiFontRegular,
                (fun l -> (Translations.Translation l).Reject),
                Vector2(100f, disclaimerHeight + 10f), // TODO: Proper x positioning
                language
            )

        member this.Draw() =
            ClearBackground(BLACK)

            match areFilesOnDisk with
            | None | Some true -> ()
            | Some false ->
                drawDisclaimer language
                acceptButton.Draw()
                rejectButton.Draw()

        member this.Load _ = disclaimerAccepted.Task
        member this.Update(input, _) =
            match areFilesOnDisk with
            | None -> if onDiskChecker.IsCompleted then areFilesOnDisk <- Some onDiskChecker.Result
            | Some _ ->
                acceptButton <- acceptButton.Update(input, language)
                rejectButton <- rejectButton.Update(input, language)
                if acceptButton.State.InteractionState = ButtonInteractionState.Clicked then
                    disclaimerAccepted.SetResult()
                else if rejectButton.State.InteractionState = ButtonInteractionState.Clicked then
                    disclaimerAccepted.SetCanceled()
