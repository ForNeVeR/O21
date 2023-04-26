namespace O21.Game

open type Raylib_CsLo.Raylib

type HUD =
    {
        Score: int
        Level: int
        Oxy: float32
        Lives: int
        Abilities: bool[]
    }
    with
        static member Init() =
            {
                Score = 0
                Level = 1
                Oxy = 0f
                Lives = 5
                Abilities = Array.init 5 (fun _ -> false ) 
            }
            
        member private this.renderBonusLine(textures: HUDSprites)  =
            for i = 1 to 5 do 
                DrawTexture(textures.Abilities[i], 11 + 17*(i-1), 365, WHITE)
                
        member private this.renderOxyLine (textures: HUDSprites) =
            DrawRectangle(254, 369, 102, 12, BLACK)
        
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
            
        member this.UpdateScore(newScore:int):HUD =
            {
                this with Score = newScore 
            }
    
        member this.UpdateOxy(newOxy:float32) =
            {
                this with Oxy = newOxy
            }
        
        member this.UpdateLives(newLives:int) =
            {
                this with Lives = newLives
            }
    
        member this.UpdateLevel(newLevel:int) =
            {
                this with Level = newLevel
            }
        
        member this.Render(textures: HUDSprites): unit =
            HUDRenderer.renderAll textures
            this.renderBonusLine textures
            this.renderOxyLine textures
            this.renderScoreLine textures
            this.renderLives textures
            this.renderLevel textures
            
            
            
            
