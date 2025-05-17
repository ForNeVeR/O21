// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open System.Numerics
open System.Threading.Tasks

open Microsoft.FSharp.Core
open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Colors
open type Raylib_CSharp.Fonts.TextManager
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

open O21.Game
open O21.Game.Localization
open O21.Game.Scenes
open Raylib_CSharp

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
    let fontSizeUnits = 16f
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
            Color.White
        )
    let mutable camera : Camera2D = Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)

    let doLayout() =
        let struct (windowWidth, windowHeight) = window.RenderTargetSize
        let screenWidth = float32 windowWidth
        let screenHeight = float32 windowHeight

        let disclaimerSize = MeasureTextEx(
            content.UiFontRegular,
            disclaimerText language,
            window.Scale fontSizeUnits,
            0f
        )
        if disclaimerSize.X > screenWidth || disclaimerSize.Y > screenHeight then
            failwith $"Disclaimer size ({disclaimerSize}) is too big to fit on the screen."

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
        let acceptButtonWidth = acceptButtonSize.Width
        let buttonTotalWidth =
            acceptButtonWidth
            + window.Scale buttonBetweenUnits
            + (rejectButton.Measure language).Width

        let totalWidth = max disclaimerSize.X buttonTotalWidth
        let totalHeight = disclaimerSize.Y + window.Scale buttonTopMarginUnits + acceptButtonSize.Height

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
        
        member this.Camera: Camera2D = camera
            
        member _.Init newContent =
            content <- newContent
            camera <- Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)

            // TODO[#98]: Update this layout on language change
            doLayout()

        member this.Draw() =
            DrawSceneHelper.configureCamera window &camera

            ClearBackground(Color.Black)

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
