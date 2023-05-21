namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95.Fish
open O21.Game.TextureUtils
open O21.Resources

type Sprites = {
    Bricks: Map<int, Texture>
    Background: Texture[]
    Fishes: Fish[]
    HUD: HUDSprites
    Bonuses: BonusSprites
}
    with
        interface IDisposable with
            member this.Dispose() =
                for t in this.Bricks.Values do
                    UnloadTexture(t)
                for t in this.Background do
                    UnloadTexture(t)

module Sprites =

    let private loadBonuses (exeGraphics: Dib[]): BonusSprites =
        let lifebuoyMasks = [|172..183|]
        let lifebuoyTextures = [|160..171|]
        let bonusesMasks = [|202; 203; 205; 206; 207; 208; 209; 210; 211; 212|]
        let bonusesTextures = [|213; 214; 216; 217; 218; 219; 220; 221; 222; 223|]
        {
            Lifebuoy = Array.init 12 (fun i ->
                CreateTransparentSprite exeGraphics[lifebuoyTextures[i]] exeGraphics[lifebuoyMasks[i]]
            )
            Static = Array.init 10 (fun i ->
                CreateTransparentSprite exeGraphics[bonusesTextures[i]] exeGraphics[bonusesMasks[i]]
            )
            LifeBonus = CreateTransparentSprite exeGraphics[215] exeGraphics[204]
        }
             
    let private loadBricks (brickGraphics: Dib[]) =
        seq { 1..9 }
        |> Seq.map(fun direction ->
            let colors = brickGraphics[direction]
            let transparency = brickGraphics[direction + 10]
            direction, CreateTransparentSprite colors transparency
        ) |> Map.ofSeq
    
    let private loadBackgrounds (backgroundGraphics: Dib[]): Texture[] =
        Array.init backgroundGraphics.Length (fun i ->
            CreateSprite backgroundGraphics[i]
        )
        
    let private createFish (index: int) (fishGraphics: Dib[]): Fish =
        let shift = index * 9
        {
            Width = fishGraphics[index * 9].Width
            Height = fishGraphics[index * 9].Height
            
            LeftDirection = Array.init 8 (fun i ->
                CreateTransparentSprite fishGraphics[shift + i + 45] fishGraphics[shift + i]
            )
            
            RightDirection = Array.init 8 (fun i ->
                CreateTransparentSprite fishGraphics[shift + i + 135] fishGraphics[shift + i + 90]
            )
            
            OnDying = [|
                  CreateTransparentSprite fishGraphics[shift + 53] fishGraphics[shift + 8];
                  CreateTransparentSprite fishGraphics[shift + 143] fishGraphics[shift + 98]
                  |]
        }
               
    let private loadFishes (fishGraphics: Dib[]): Fish[] =
        Array.init (fishGraphics.Length / 36) ( fun i ->
            createFish i fishGraphics
        )
        
    let private loadHUD (exeGraphics: Dib[]): HUDSprites =
       let hud = [| 26; 27; 29; 185; 159;|]
       let controls = [| 5; 25; 28; 184; 224; |]
       let abilities = [| 186..191 |]
       let digits = [| 192..201 |]
       {
           Abilities = Array.init abilities.Length (fun i ->
               CreateSprite exeGraphics[abilities[i]]
           )
           HUDElements = Array.init hud.Length (fun i ->
               CreateSprite exeGraphics[hud[i]]
           )
           Digits = Array.init digits.Length (fun i ->
               CreateSprite exeGraphics[digits[i]]    
           )
           Controls = Array.init controls.Length (fun i ->
               CreateSprite exeGraphics[controls[i]]    
           )
       }

    let LoadFrom (directory: string): Task<Sprites> = task {
        let brickResources = Graphics.Load(Path.Combine(directory, "U95_BRIC.DLL"))
        
        let fishes = Graphics.Load(Path.Combine(directory, "U95_PIC.DLL"))
        
        let exeSprites = Graphics.Load(Path.Combine(directory, "U95.EXE"))
        
        let backgrounds = Background.LoadBackgrounds(directory)
        
        return {
            Bricks = loadBricks brickResources
            Background = loadBackgrounds backgrounds
            Fishes = loadFishes fishes
            HUD = loadHUD exeSprites
            Bonuses = loadBonuses exeSprites
        }
    }
