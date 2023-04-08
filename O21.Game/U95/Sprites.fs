namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game
open O21.Game.U95.Fish
open O21.Resources

type Sprites = {
    Bricks: Map<int, Texture2D>
    Background: Texture2D[]
    Fishes: Fish[]
    HUD: Texture2D[]
}
    with
        interface IDisposable with
            member this.Dispose() =
                for t in this.Bricks.Values do
                    t.Dispose()
                for t in this.Background do
                    t.Dispose()

module Sprites =

    let private transparentColor = struct(0xFFuy, 0xFFuy, 0xFFuy)
    let private isColor(struct(r1, g1, b1), struct(r2, g2, b2)) =
        r1 = r2 && g1 = g2 && b1 = b2

    let private createSprite device (colors: Dib) (transparency: Dib) =
        let width = colors.Width
        let height = colors.Height
        let texture = new Texture2D(device, width, height)
        let colors = Array.init (width * height) (fun i ->
            let x = i % width
            let y = i / width
            let isTransparent = isColor(transparency.GetPixel(x, y), transparentColor)
            if isTransparent then
                Color.Transparent
            else
                let struct(r, g, b) = colors.GetPixel(x, y)
                Color(
                    r = int r,
                    g = int g,
                    b = int b
                )
        )
        texture.SetData colors
        texture

    let CreateSprite device (colors: Dib) =
        let width = colors.Width
        let height = colors.Height
        let texture = new Texture2D(device, width, height)
        let colors = Array.init (width * height) (fun i ->
            let x = i % width
            let y = i / width
            let struct(r, g, b) = colors.GetPixel(x, y)
            Color(
                r = int r,
                g = int g,
                b = int b
            )
        )
        texture.SetData colors
        texture

    let private loadBricks device (brickGraphics: Dib[]) =
        seq { 1..9 }
        |> Seq.map(fun direction ->
            let colors = brickGraphics[direction]
            let transparency = brickGraphics[direction + 10]
            direction, createSprite device colors transparency
        ) |> Map.ofSeq
    
    let private loadBackgrounds device (backgroundGraphics: Dib[]): Texture2D[] =
        Array.init backgroundGraphics.Length (fun i ->
            createSprite device backgroundGraphics[i] backgroundGraphics[i]
        )
        
    let private createFish (index: int) device (fishGraphics: Dib[]): Fish =
        let shift = index * 9
        {
            Width = fishGraphics[index * 9].Width
            Height = fishGraphics[index * 9].Height
            
            LeftDirection = Array.init 8 (fun i ->
                createSprite device fishGraphics[shift + i + 45] fishGraphics[shift + i]
            )
            
            RightDirection = Array.init 8 (fun i ->
                createSprite device fishGraphics[shift + i + 135] fishGraphics[shift + i + 90] 
            )
            
            OnDying = [|
                  createSprite device fishGraphics[shift + 53] fishGraphics[shift + 8];
                  createSprite device fishGraphics[shift + 143] fishGraphics[shift + 98]
                  |]
        }
    
    let private loadFishes device (fishGraphics: Dib[]): Fish[] =
        Array.init (fishGraphics.Length / 36) ( fun i ->
            createFish i device fishGraphics
        )
        
    let private loadHUD device (hudGraphics: Dib[]) =
        Array.init (hudGraphics.Length) ( fun i ->
            createSprite device hudGraphics[i] hudGraphics[i]   
        )

    let LoadFrom(device: GraphicsDevice) (directory: string): Task<Sprites> = task {
        let brickResources = Graphics.Load(Path.Combine(directory, "U95_BRIC.DLL"))
        
        let fishes = Graphics.Load(Path.Combine(directory, "U95_PIC.DLL"))
        
        let exeSprites = Graphics.Load(Path.Combine(directory, "U95.EXE"))
        
        let hudSprites = HUDSprites.Load exeSprites
        
        let backgrounds = Background.LoadBackgrounds(directory)
        
        return {
            Bricks = loadBricks device brickResources
            Background = loadBackgrounds device backgrounds
            Fishes = loadFishes device fishes
            HUD = loadHUD device hudSprites
        }
    }
