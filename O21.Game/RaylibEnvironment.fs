module O21.Game.RaylibEnvironment

open Raylib_CsLo
open type Raylib_CsLo.Raylib

let private AdjustWindowScale(currentWidth, currentHeight) =
    let scale = GetWindowScaleDPI()
    SetWindowSize(int <| float32 currentWidth * scale.X, int <| float32 currentHeight * scale.Y)

let private AccommodateWindowSize monitor =
    let currentWidth, currentHeight = Raylib.GetScreenWidth(), Raylib.GetScreenHeight()
    let monitorWidth, monitorHeight = Raylib.GetMonitorWidth monitor, Raylib.GetMonitorHeight monitor
    let allowedWidth, allowedHeight = min currentWidth monitorWidth, min currentHeight monitorHeight
    if allowedWidth <> currentWidth || allowedHeight <> currentHeight then
        SetWindowSize(allowedWidth, allowedHeight)

let private InitializeWindow windowSize =
    let initialWidth, initialHeight =
        match windowSize with
        | Some(struct(w, h)) -> w, h
        | None ->
            let struct(w, h) = WindowParameters.DefaultWindowSizeWithoutScale
            w, h

    InitWindow(initialWidth, initialHeight, "O21")
    if Option.isNone windowSize then
        AdjustWindowScale(initialWidth, initialHeight)

    AccommodateWindowSize(GetCurrentMonitor())
    WindowParameters.Init()

let Run(windowSize: Option<struct(int*int)>, play: WindowParameters -> unit): unit =
    SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE)
    let window = InitializeWindow windowSize
    InitAudioDevice()
    SetAudioStreamBufferSizeDefault Music.BufferSize

    SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT)
    try
        play window
    finally
        CloseAudioDevice()
        CloseWindow()
