namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game
open O21.Game.U95
open O21.Game.U95.Parser

type PlayScene() =

    let wantShot(input: Input) = input.Pressed |> List.contains Key.Fire
    let allowedShot (world: GameWorld) (time: Time) =
        match world.LastShotTime with
        | None -> true
        | Some lastShot -> time.Total - lastShot > GameRules.ShotCooldownSec

    let shot (world: GameWorld) (time: Time) =
        { world with
            LastShotTime = Some time.Total
            SoundsToStartPlaying = world.SoundsToStartPlaying |> Set.add SoundType.Shot }

    interface IGameScene with
        member this.Update world input time =
            if wantShot input && allowedShot world time then
                shot world time
            else
                world

        member _.Render (batch: SpriteBatch) (gameData: U95Data) (world: GameWorld) =
            batch.Draw(gameData.Sprites.Background[1], Rectangle(0, 0, 600, 300), Color.White)         
            let hud = HUD()
            hud.Init(gameData.Sprites.HUD, batch)
            hud.UpdateScore(100, batch)
            let map = world.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        batch.Draw(gameData.Sprites.Bricks[b], Rectangle(12*j, 12*i, 12, 12), Color.White)
                    | _ ->
                        ()
            for i = 0 to gameData.Sprites.Fishes.Length-1 do
                batch.Draw(gameData.Sprites.Fishes[i].LeftDirection[i], Rectangle(60*i, 60*i,
                                                                        gameData.Sprites.Fishes[i].Width,
                                                                        gameData.Sprites.Fishes[i].Height), Color.White)
