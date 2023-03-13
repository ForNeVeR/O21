namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Resources

type Sprites = {
    Bricks: Map<int, Texture2D>
    Background: Texture2D[]
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
         
    let LoadFrom(device: GraphicsDevice) (directory: string): Task<Sprites> = task {
        let brickResources = Graphics.Load(Path.Combine(directory, "U95_BRIC.DLL"))
        let backgrounds = Background.LoadBackgrounds(directory)
        
        return {
            Bricks = loadBricks device brickResources
            Background = loadBackgrounds device backgrounds
        }
    }
