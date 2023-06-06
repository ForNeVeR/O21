module O21.Game.RaylibEnvironment

open Raylib_CsLo
open type Raylib_CsLo.Raylib

let Run(config: Config, play: unit -> unit): unit =
    InitWindow(config.ScreenWidth, config.ScreenHeight, config.Title)

    InitAudioDevice()
    SetAudioStreamBufferSizeDefault Music.BufferSize

    SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT)
    try
        play()
    finally
        CloseAudioDevice()
        CloseWindow()
