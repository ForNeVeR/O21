namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95
open O21.Game.U95.Parser

type PlayScene = {
    CurrentLevel: Level
    LastShotTime: float option
} with

    static member Init(level: Level): PlayScene = {
        CurrentLevel = level
        LastShotTime = None
    }

    interface IScene with
        member this.Update(input, time, state) =
            let wantShot = input.Pressed |> List.contains Key.Fire

            let allowedShot =
                match this.LastShotTime with
                | None -> true
                | Some lastShot -> time.Total - float lastShot > GameRules.ShotCooldownSec

            if wantShot && allowedShot then
                { state with 
                    Scene = { this with LastShotTime = Some time.Total }
                    SoundsToStartPlaying = state.SoundsToStartPlaying |> Set.add SoundType.Shot
                }
            else
                state
 
        member this.Draw(state: State) =           
            DrawTexture(state.U95Data.Sprites.Background[1], 0, 0, WHITE)       
            let hud = HUD()
            hud.Init(state.U95Data.Sprites.HUD)
            hud.UpdateScore(100)
            let map = this.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(state.U95Data.Sprites.Bricks[b], 12*j, 12*i, WHITE)
                    | _ ->
                        ()
            for i = 0 to state.U95Data.Sprites.Fishes.Length-1 do
                DrawTexture(state.U95Data.Sprites.Fishes[i].LeftDirection[i], 60*i, 60*i, WHITE)
