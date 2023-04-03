namespace O21.Game

open Microsoft.Xna.Framework.Graphics

type HUD() =
    
    let mutable score = 0
    let mutable level = 1
    let mutable oxy = 0f
    let mutable lives = 0
    let mutable abilities = Array.init 5 ( fun _ -> false) 
    
    member _.UpdateScore(newScore:int, batch: SpriteBatch) =
        score <- newScore
        HUDRenderer.renderScoreLine batch score
    
    member _.UpdateOxy(newOxy:float32, batch:SpriteBatch) =
        oxy <- newOxy
        HUDRenderer.renderOxyLine batch oxy
        
    member _.UpdateLives(newLives:int, batch:SpriteBatch) =
        lives <- newLives
        HUDRenderer.renderLevel batch lives
    
    member _.UpdateLevel(newLevel:int, batch:SpriteBatch) =
        level <- newLevel
        HUDRenderer.renderLevel batch level
        
    member _.Init(sprites: Texture2D[], batch: SpriteBatch) =
       HUDRenderer.renderAll sprites batch
