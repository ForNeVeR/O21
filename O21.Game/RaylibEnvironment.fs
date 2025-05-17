// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.RaylibEnvironment

open Raylib_CSharp.Audio
open Raylib_CSharp.Interact
open Raylib_CSharp.Windowing
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Interact.Input
open type Raylib_CSharp.Fonts.TextManager
open type Raylib_CSharp.Rendering.Graphics

let private AdjustWindowScale(currentWidth, currentHeight) =
    let scale = Window.GetScaleDPI()
    Window.SetSize(int <| float32 currentWidth * scale.X, int <| float32 currentHeight * scale.Y)

let private AccommodateWindowSize monitor =
    let currentWidth, currentHeight = Window.GetScreenWidth(), Window.GetScreenHeight()
    let monitorWidth, monitorHeight = Window.GetMonitorWidth monitor, Window.GetMonitorHeight monitor
    let allowedWidth, allowedHeight = min currentWidth monitorWidth, min currentHeight monitorHeight
    if allowedWidth <> currentWidth || allowedHeight <> currentHeight then
        Window.SetSize(allowedWidth, allowedHeight)

let private InitializeWindow windowSize =
    let initialWidth, initialHeight =
        match windowSize with
        | Some(struct(w, h)) -> w, h
        | None ->
            let struct(w, h) = WindowParameters.DefaultWindowSizeWithoutScale
            w, h

    Window.Init(initialWidth, initialHeight, "O21")
    if Option.isNone windowSize then
        AdjustWindowScale(initialWidth, initialHeight)

    AccommodateWindowSize(Window.GetCurrentMonitor())
    WindowParameters.Init()

let Run(windowSize: Option<struct(int*int)>, play: WindowParameters -> unit): unit =
    SetConfigFlags(ConfigFlags.ResizableWindow)
    let window = InitializeWindow windowSize
    AudioDevice.Init()
    AudioStream.SetBufferSizeDefault(Music.BufferSize)

    SetMouseCursor(MouseCursor.Default)
    try
        play window
    finally
        AudioDevice.Close()
        Window.Close()
