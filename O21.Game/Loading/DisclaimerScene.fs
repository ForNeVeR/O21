namespace O21.Game.Loading

open System.Numerics
open System.Threading.Tasks

open Microsoft.FSharp.Core
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Localization
open O21.Game.Scenes

type DisclaimerScene(config: Config) =
    let onDiskChecker = task {
        let! contentConfig = Downloader.LoadContentConfiguration()
        return! Downloader.CheckIfAlreadyLoaded contentConfig config.U95DataDirectory
    }
    let mutable areFilesOnDisk = None

    // TODO[#98]: Localization controls on this page
    let language = Translations.DefaultLanguage
    let disclaimerAccepted = TaskCompletionSource<unit>()

    let mutable content = Unchecked.defaultof<_>
    let mutable disclaimerPosition = Vector2.Zero
    let fontSizePx = 24f
    let buttonTopMarginPx = 25f
    let buttonBetweenPx = 100f
    let mutable acceptButton = Unchecked.defaultof<Button>
    let mutable rejectButton = Unchecked.defaultof<Button>

    let disclaimerText lang = (Translations.Translation lang).LegalDisclaimerText
    let drawDisclaimer lang =
        DrawTextEx(
            content.UiFontRegular,
            disclaimerText lang,
            disclaimerPosition,
            fontSizePx,
            0f,
            WHITE
        )

    let doLayout() =
        let screenWidth = float32 config.ScreenWidth
        let screenHeight = float32 config.ScreenHeight

        let disclaimerSize = MeasureTextEx(
            content.UiFontRegular,
            disclaimerText language,
            float32 fontSizePx,
            0f
        )
        if disclaimerSize.X > screenWidth || disclaimerSize.Y > screenHeight then
            failwith $"Disclaimer size ({disclaimerSize}) is too big to fit on the screen."

        acceptButton <- Button.Create(
            content.UiFontRegular,
            (fun l -> (Translations.Translation l).Accept),
            Vector2.Zero,
            language
        )
        rejectButton <- Button.Create(
            content.UiFontRegular,
            (fun l -> (Translations.Translation l).Reject),
            Vector2.Zero,
            language
        )

        let acceptButtonSize = acceptButton.Measure language
        let acceptButtonWidth = acceptButtonSize.width
        let buttonTotalWidth = acceptButtonWidth + buttonBetweenPx + (rejectButton.Measure language).width

        let totalWidth = max disclaimerSize.X buttonTotalWidth
        let totalHeight = disclaimerSize.Y + buttonTopMarginPx + acceptButtonSize.height

        disclaimerPosition <- Vector2(screenWidth / 2f - totalWidth / 2f, screenHeight / 2f - totalHeight / 2f)
        let buttonTopPx = disclaimerPosition.Y + disclaimerSize.Y + buttonTopMarginPx
        acceptButton <- {
            acceptButton with
                Position = Vector2(screenWidth / 2f - buttonTotalWidth / 2f, buttonTopPx)
        }
        rejectButton <- {
            rejectButton with
                Position = Vector2(acceptButton.Position.X + acceptButtonWidth + buttonBetweenPx, buttonTopPx)
        }


    interface ILoadingScene<LocalContent, unit> with
        member _.Init newContent =
            content <- newContent
            // TODO[#98]: Update this layout on language change
            doLayout()

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
            | Some true ->
                disclaimerAccepted.SetResult()
            | Some false ->
                acceptButton <- acceptButton.Update(input, language)
                rejectButton <- rejectButton.Update(input, language)
                if acceptButton.State.InteractionState = ButtonInteractionState.Clicked then
                    disclaimerAccepted.SetResult()
                else if rejectButton.State.InteractionState = ButtonInteractionState.Clicked then
                    disclaimerAccepted.SetCanceled()
