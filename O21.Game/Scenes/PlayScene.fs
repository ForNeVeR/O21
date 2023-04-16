namespace O21.Game.Scenes

open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95
open O21.Game.U95.Parser

type PlayScene() =

    let wantShot (input: Input) = input.Pressed |> List.contains Key.Fire

    let allowedShot (world: GameWorld) (time: Time) =
        match world.LastShotTime with
        | None -> true
        | Some lastShot -> time.Total - float lastShot > GameRules.ShotCooldownSec

    let shot (world: GameWorld) (time: Time) = {
        world with
            LastShotTime = Some time.Total
            SoundsToStartPlaying = world.SoundsToStartPlaying |> Set.add SoundType.Shot
    }

    interface IGameScene with
        member _.Update world input time =
            if wantShot input && allowedShot world time then
                shot world time
            else
                world

        member _.Render (gameData: U95Data) (world: GameWorld) =
            DrawTexture(gameData.Sprites.Background[1], 0, 0, WHITE)
            let hud = HUD()
            hud.Init(gameData.Sprites.HUD)
            hud.UpdateScore(100)
            let map = world.CurrentLevel.LevelMap

            for i = 0 to map.Length - 1 do
                for j = 0 to map[i].Length - 1 do
                    match map[i][j] with
                    | Brick b -> DrawTexture(gameData.Sprites.Bricks[b], 12 * j, 12 * i, WHITE)
                    | _ -> ()

            for i = 0 to gameData.Sprites.Fishes.Length - 1 do
                DrawTexture(gameData.Sprites.Fishes[i].LeftDirection[i], 60 * i, 60 * i, WHITE)
