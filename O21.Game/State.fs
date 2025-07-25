// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open O21.Game.Engine
open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.U95
open Raylib_CSharp.Camera.Cam2D

type Settings = { 
    SoundVolume: float32
}

[<RequireQualifiedAccess>]
type Scene =
    | MainMenu
    | Play
    | GameOver
    | Help
    
type SceneEvent = NavigateTo of Scene

type IScene =
    abstract member Update: Input * Instant * State -> State * SceneEvent option
    abstract member Draw: State -> unit
    abstract member RenderTargetSize: struct(int * int) with get

and State = {
    Scene: IScene
    Settings: Settings
    U95Data: U95Data
    SoundsToStartPlaying: Set<SoundType>
    Language: Language
    Engine: TickEngine
    MusicPlayer: MusicPlayer option
    Camera: Camera2D
}
