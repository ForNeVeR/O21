namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

module HUDRenderer = 
        let palette = [|
            Color(192, 192, 192); // main color 
            Color(108, 108, 108) // texture frame
            Color(207, 207, 207) // block frame
        |]
        let mutable sprites:Texture2D[] = Array.empty
        
        let createRectangle (width:int) (height:int) (device: GraphicsDevice) (color:Color): Texture2D =
            let rectangleTexture = new Texture2D(device, width, height)
            let colorData = Array.init (width * height) (fun _ -> color)
            rectangleTexture.SetData(colorData)
            rectangleTexture 
    
        let renderOxyLine(batch: SpriteBatch) (oxy: float32) =
            batch.Draw(createRectangle 102 12 batch.GraphicsDevice Color.Black , Vector2(254f, 369f), Color.White)
        
        let renderScoreLine(batch: SpriteBatch) (score: int) =
            let mutable tmp = score
            for i = 6 downto 0 do
                batch.Draw(sprites[tmp % 10 + 9], Vector2(128f + float32 (13 * i), 350f), Color.White)
                tmp <- tmp / 10
            
        let renderBonusLine(batch: SpriteBatch)  =
            let cell = createRectangle 17 17 batch.GraphicsDevice Color.Black
            for i = 0 to 4 do 
                batch.Draw(cell, Vector2(11f + float32 (i * 17), 365f), Color.White)
        
        let renderLevel(batch: SpriteBatch) (level: int) =
            let mutable tmp = level
            for i = 1 downto 0 do
                batch.Draw(sprites[tmp % 10 + 9], Vector2(68f + float32 (13 * i), 325f), Color.White)
                tmp <- tmp / 10
        
        let renderLives(batch: SpriteBatch) (lives: int) =
            batch.Draw( sprites[lives % 10 + 9], Vector2(314f,320f), Color.White) // what if the number of lives is a two-digit number?
        
        let private drawFrame (position:Vector2) (width:int) (height:int) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
            let left = createRectangle 1 (height + 4) batch.GraphicsDevice palette[1] 
            let right = createRectangle 1 (height + 4) batch.GraphicsDevice Color.White
            let up = createRectangle (width + 4) 1 batch.GraphicsDevice palette[1]
            let down = createRectangle (width + 4) 1 batch.GraphicsDevice Color.White
        
            batch.Draw(left, Vector2(x - 2f, y - 2f), Color.White)
            batch.Draw(right, Vector2(x + float32 width + 2f, y - 2f), Color.White)
            batch.Draw(up, Vector2(x - 2f, y - 2f), Color.White)
            batch.Draw(down, Vector2(x - 1f, y + float32 height + 2f), Color.White)
        
        let private drawBlockFrame (position:Vector2) (width:int) (height:int) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
            let left = createRectangle 1 height batch.GraphicsDevice Color.White
            let right = createRectangle 1 height batch.GraphicsDevice palette[2]
            let up = createRectangle width 1 batch.GraphicsDevice Color.White
            let down = createRectangle width 1 batch.GraphicsDevice palette[2]
        
            batch.Draw(left, Vector2(x, y), Color.White)
            batch.Draw(right, Vector2(x + float32 width, y), Color.White)
            batch.Draw(up, Vector2(x, y), Color.White)
            batch.Draw(down, Vector2(x, y + float32 height - 1f), Color.White)
      
        let private renderFirstBlock(position:Vector2) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
            let block = createRectangle 106 80 batch.GraphicsDevice palette[0]
            batch.Draw(block, Vector2(x, y + 10f), Color.White)
            batch.Draw(sprites[5], Vector2(x + 16f, y + 27f), Color.White)
            renderLevel batch 1
       
            drawFrame (Vector2(x + 68f, y + 25f)) 26 23 batch
            drawFrame (Vector2(x + 11f, y + 65f)) 85 17 batch
            drawBlockFrame (Vector2(0f, y + 10f)) 106 80 batch
            renderBonusLine batch
    
        let private renderSecondBlock(position:Vector2) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
        
            let block = createRectangle 135 80 batch.GraphicsDevice palette[0]
            batch.Draw(block, Vector2(x, y + 10f), Color.White)
            drawBlockFrame (Vector2(x, y + 10f)) 135 80 batch
            batch.Draw(sprites[7], Vector2(x + 41f, y + 20f), Color.White)
            drawFrame (Vector2(x + 21f, y + 50f)) 91 23 batch
        
            renderScoreLine batch 0
    
        let private renderThirdBlock(position:Vector2) (batch: SpriteBatch) =
            let x,y = position.X, position.Y

            let block = createRectangle 126 80 batch.GraphicsDevice palette[0]
            batch.Draw(block, Vector2(x, y + 10f), Color.White)        
            drawBlockFrame (Vector2(x, y + 10f)) 126 80 batch
            batch.Draw(sprites[2], Vector2(x + 32f, y + 20f), Color.White)
            drawFrame (Vector2(x + 32f, y + 20f)) 52 23 batch
            batch.Draw(sprites[20], Vector2(x+32f, y + 48f), Color.White)
        
            renderLives batch 1
            renderOxyLine batch 0f
    
        let private renderFourthBlock(position:Vector2) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
        
            let block = createRectangle 94 80 batch.GraphicsDevice palette[0] 
            batch.Draw(block, Vector2(x,y + 10f), Color.White)
            drawBlockFrame (Vector2(x, y + 10f)) 94 80 batch  
            batch.Draw(sprites[4], Vector2(x + 10f, y + 38f), Color.White)
            drawBlockFrame (Vector2(x + 10f, y + 38f)) 23 23 batch
            batch.Draw(sprites[1], Vector2(x + 33f, y + 38f), Color.White)
            drawBlockFrame (Vector2(x + 33f, y + 38f)) 23 23 batch 
            batch.Draw(sprites[6], Vector2(x + 56f, y + 38f), Color.White)
            drawBlockFrame (Vector2(x + 56f, 338f)) 23 23 batch
            batch.Draw(sprites[19], Vector2(x + 33f, y + 14f), Color.White)
            drawBlockFrame (Vector2(x + 33f, 315f)) 23 23 batch
            batch.Draw(sprites[0], Vector2(x + 33f, y + 61f), Color.White)
            drawBlockFrame (Vector2(x + 33f, y + 61f)) 23 23 batch
    
        let private renderFifthBlock(position:Vector2) (batch: SpriteBatch) =
            let x,y = position.X, position.Y
            let block = createRectangle 134 80 batch.GraphicsDevice palette[0]
            batch.Draw(block, Vector2(x, y+10f), Color.White)
            drawBlockFrame (Vector2(x, y + 10f)) 134 80 batch 
            batch.Draw(sprites[3], Vector2(x + 21f, y + 22f), Color.White)
        
        let renderAll(graphics:Texture2D[]) (batch: SpriteBatch) =
            
            let mutable x = 0f
            let mutable y = 300f
            sprites <- graphics
            
            batch.Draw(createRectangle 600 100 batch.GraphicsDevice Color.Gray, Vector2(x,y), Color.White) 
            batch.Draw(createRectangle 600 2 batch.GraphicsDevice palette[1], Vector2(x + 1f, y), Color.White)
        
            renderFirstBlock (Vector2(x,y)) batch
            x <- x + 1f + 106f
            renderSecondBlock (Vector2(x,y)) batch
            x <- x + 135f + 1f
            renderThirdBlock (Vector2(x,y)) batch  
            x <- x + 126f + 1f
            renderFourthBlock (Vector2(x,y)) batch
            x <- x + 94f + 1f
            renderFifthBlock (Vector2(x,y)) batch
