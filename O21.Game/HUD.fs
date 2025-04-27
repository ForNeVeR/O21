// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System
open O21.Game.Engine
open Raylib_CsLo
open type Raylib_CsLo.Raylib
    
type HUD =
    {
        Score: int
        Level: int
        Oxy: int
        Lives: int
        Abilities: bool[]
        Pause: bool
        Controls: Controls
    }
    with
        static member Init() =
            {
                Score = 0
                Level = 1
                Oxy = 0
                Lives = 5
                Abilities = Array.init 5 (fun _ -> false )
                Pause = false 
                Controls = Controls.Init()
            }
            
        member private this.renderBonusLine(textures: HUDSprites)  =
            for i = 1 to 5 do 
                DrawTexture(textures.Abilities[i], 11 + 17*(i-1), 365, WHITE)
                
        member private this.renderOxyLine (textures: HUDSprites) =
            let blue = Color(0, 0, 255, 255)
            DrawRectangle(254, 369, 102, 12, BLACK)
            DrawRectangle(255, 370, this.Oxy, 10, blue)
        
        member private this.renderScoreLine (textures: HUDSprites) =
            let mutable tmp = this.Score
            for i = 6 downto 0 do
                DrawTexture(textures.Digits[tmp % 10], 128 + 13*i, 350, WHITE)
                tmp <- tmp / 10
        
        member private this.renderLevel (textures: HUDSprites) =
            let mutable tmp = this.Level
            for i = 1 downto 0 do
                DrawTexture(textures.Digits[tmp % 10], 68 + 13*i, 325, WHITE)
                tmp <- tmp / 10        
        
        member private this.renderLives (textures: HUDSprites) =
            DrawTexture(textures.Digits[this.Lives % 10], 314, 320, WHITE) // what if the number of lives is a two-digit number?
            
        member private this.renderPause (texture: Texture) =
            if this.Pause then 
                DrawTexture(texture, 262, 150, WHITE)
            
        member this.SyncWithGame(gameEngine: GameEngine) =
            { this with
                Lives = gameEngine.Player.Lives
                Score = gameEngine.Player.Score
                Oxy = gameEngine.Player.OxygenAmount
                Pause = not gameEngine.IsActive
                Level = gameEngine.CurrentLevel.Position
            }
            
        member this.UpdateScore(newScore:int):HUD =
            {
                this with Score = newScore 
            }
    
        member this.UpdateOxy(newOxy:int) =
            {
                this with Oxy = Math.Clamp(newOxy, 0, 100)
            }
        
        member this.UpdateLives(newLives:int) =
            {
                this with Lives = newLives
            }
    
        member this.UpdateLevel(newLevel:int) =
            {
                this with Level = newLevel
            }
        
        member this.Render(textures: HUDSprites, content: LocalContent): unit =
            HUDRenderer.renderAll textures
            this.renderBonusLine textures
            this.renderOxyLine textures
            this.renderScoreLine textures
            this.renderLives textures
            this.renderLevel textures
            this.Controls.Render(textures)
            this.renderPause content.PauseTexture
