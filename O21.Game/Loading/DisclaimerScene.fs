namespace O21.Game.Loading

open System.Numerics
open System.Threading.Tasks

open Microsoft.FSharp.Core
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Localization
open O21.Game.Scenes
open Raylib_CsLo

type DisclaimerScene(window: WindowParameters, u95DataDirectory: string) =
    let onDiskChecker = task {
        let! contentConfig = Downloader.LoadContentConfiguration()
        return! Downloader.CheckIfAlreadyLoaded contentConfig u95DataDirectory
    }
    let mutable areFilesOnDisk = None

    // TODO[#98]: Localization controls on this page
    let language = Translations.DefaultLanguage
    let disclaimerAccepted = TaskCompletionSource<unit>()

    let mutable content = Unchecked.defaultof<_>
    let mutable disclaimerPosition = Vector2.Zero
    let fontSizeUnits = 24f
    let buttonTopMarginUnits = 25f
    let buttonBetweenUnits = 100f
    let mutable acceptButton = Unchecked.defaultof<Button>
    let mutable rejectButton = Unchecked.defaultof<Button>

    let disclaimerText lang = (Translations.Translation lang).LegalDisclaimerText
    let drawDisclaimer lang =
        DrawTextEx(
            content.UiFontRegular,
            disclaimerText lang,
            disclaimerPosition,
            window.Scale fontSizeUnits,
            0f,
            WHITE
        )
    let mutable camera : Camera2D = Camera2D(zoom = 1f)

    let doLayout() =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let screenWidth = float32 windowWidth
        let screenHeight = float32 windowHeight

        let disclaimerSize = MeasureTextEx(
            content.UiFontRegular,
            disclaimerText language,
            window.Scale fontSizeUnits,
            0f
        )
        // TODO[#145]: Uncomment this after we support proper scaling of this window, with camera/zoom and all the stuff.
        // if disclaimerSize.X > screenWidth || disclaimerSize.Y > screenHeight then
            // failwith $"Disclaimer size ({disclaimerSize}) is too big to fit on the screen."

        acceptButton <- Button.Create(
            window,
            content.UiFontRegular,
            (fun l -> (Translations.Translation l).Accept),
            Vector2.Zero,
            language
        )
        rejectButton <- Button.Create(
            window,
            content.UiFontRegular,
            (fun l -> (Translations.Translation l).Reject),
            Vector2.Zero,
            language
        )

        let acceptButtonSize = acceptButton.Measure language
        let acceptButtonWidth = acceptButtonSize.width
        let buttonTotalWidth =
            acceptButtonWidth
            + window.Scale buttonBetweenUnits
            + (rejectButton.Measure language).width

        let totalWidth = max disclaimerSize.X buttonTotalWidth
        let totalHeight = disclaimerSize.Y + window.Scale buttonTopMarginUnits + acceptButtonSize.height

        disclaimerPosition <- Vector2(screenWidth / 2f - totalWidth / 2f, screenHeight / 2f - totalHeight / 2f)
        let buttonTopPx = disclaimerPosition.Y + disclaimerSize.Y + window.Scale buttonTopMarginUnits
        acceptButton <- {
            acceptButton with
                Position = Vector2(screenWidth / 2f - buttonTotalWidth / 2f, buttonTopPx)
        }
        rejectButton <- {
            rejectButton with
                Position = Vector2(
                    acceptButton.Position.X + acceptButtonWidth + window.Scale buttonBetweenUnits,
                    buttonTopPx
                )
        }


    interface ILoadingScene<LocalContent, unit> with
        
        member this.Camera: Raylib_CsLo.Camera2D = camera
            
        member _.Init newContent =
            content <- newContent
            camera <- Camera2D(zoom = 1f)

            // TODO[#98]: Update this layout on language change
            doLayout()

        member this.Draw() =
            let struct (windowWidth, windowHeight) = window.WindowSizePx
            let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize

            let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * camera.zoom) / -2f / camera.zoom
            let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * camera.zoom) / -2f / camera.zoom
            
            camera.target <- Vector2(cameraTargetX, cameraTargetY)
            camera.zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                                    ((windowWidth |> float32) / (renderTargetWidth |> float32))

            ClearBackground(BLACK)

            match areFilesOnDisk with
            | None | Some true -> ()
            | Some false ->
                drawDisclaimer language
                acceptButton.Draw()
                rejectButton.Draw()

        member this.Load(_, _) = disclaimerAccepted.Task
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
