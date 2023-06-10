namespace O21.Game.U95

open System.IO
open System.Threading.Tasks

open JetBrains.Lifetimes
open Oddities.Resources
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95.Fish
open O21.Game.TextureUtils

type PlayerSprites =
    {
        Left: Texture[]
        Right: Texture[]
    }

    static member Load(lifetime: Lifetime) (images: Dib[]): PlayerSprites =
        let loadImageByIndex i =
            CreateTransparentSprite lifetime images[i] images[i + 24]
        let right = seq { 6; yield! [| 17..23 |] } |> Seq.map loadImageByIndex |> Seq.toArray
        let left = seq { yield! [| 7..13 |]; 24 } |> Seq.map loadImageByIndex |> Seq.toArray
        { Left = left; Right = right }

type Sprites = {
    Bricks: Map<int, Texture>
    Background: Texture[]
    TitleScreenBackground: Texture
    Fishes: Fish[]
    Player: PlayerSprites
    HUD: HUDSprites
    Bonuses: BonusSprites
}

module Sprites =

    let private loadBonuses lt (exeGraphics: Dib[]): BonusSprites =
        let lifebuoyMasks = [|172..183|]
        let lifebuoyTextures = [|160..171|]
        let bonusesMasks = [|202; 203; 205; 206; 207; 208; 209; 210; 211; 212|]
        let bonusesTextures = [|213; 214; 216; 217; 218; 219; 220; 221; 222; 223|]
        {
            Lifebuoy = Array.init 12 (fun i ->
                CreateTransparentSprite lt exeGraphics[lifebuoyTextures[i]] exeGraphics[lifebuoyMasks[i]]
            )
            Static = Array.init 10 (fun i ->
                CreateTransparentSprite lt exeGraphics[bonusesTextures[i]] exeGraphics[bonusesMasks[i]]
            )
            LifeBonus = CreateTransparentSprite lt exeGraphics[215] exeGraphics[204]
        }
             
    let private loadBricks lt (brickGraphics: Dib[]) =
        seq { 1..9 }
        |> Seq.map(fun direction ->
            let colors = brickGraphics[direction]
            let transparency = brickGraphics[direction + 10]
            direction, CreateTransparentSprite lt colors transparency
        ) |> Map.ofSeq
    
    let private loadBackgrounds lt (backgroundGraphics: Dib[]): Texture[] =
        Array.init backgroundGraphics.Length (fun i ->
            CreateSprite lt backgroundGraphics[i]
        )
        
    let private createFish lt (index: int) (fishGraphics: Dib[]): Fish =
        let shift = index * 9
        {
            Width = fishGraphics[index * 9].Width
            Height = fishGraphics[index * 9].Height
            
            LeftDirection = Array.init 8 (fun i ->
                CreateTransparentSprite lt fishGraphics[shift + i + 45] fishGraphics[shift + i]
            )
            
            RightDirection = Array.init 8 (fun i ->
                CreateTransparentSprite lt fishGraphics[shift + i + 135] fishGraphics[shift + i + 90]
            )
            
            OnDying = [|
                  CreateTransparentSprite lt fishGraphics[shift + 53] fishGraphics[shift + 8];
                  CreateTransparentSprite lt fishGraphics[shift + 143] fishGraphics[shift + 98]
                  |]
        }
               
    let private loadFishes lt (fishGraphics: Dib[]): Fish[] =
        Array.init (fishGraphics.Length / 36) ( fun i ->
            createFish lt i fishGraphics
        )
        
    let private loadHUD lt (exeGraphics: Dib[]): HUDSprites =
       let hud = [| 26; 27; 29; 185; 159;|]
       let controls = [| 5; 25; 28; 184; 224; |]
       let abilities = [| 186..191 |]
       let digits = [| 192..201 |]
       {
           Abilities = Array.init abilities.Length (fun i ->
               CreateSprite lt exeGraphics[abilities[i]]
           )
           HUDElements = Array.init hud.Length (fun i ->
               CreateSprite lt exeGraphics[hud[i]]
           )
           Digits = Array.init digits.Length (fun i ->
               CreateSprite lt exeGraphics[digits[i]]
           )
           Controls = Array.init controls.Length (fun i ->
               CreateSprite lt exeGraphics[controls[i]]
           )
       }

    let LoadFrom (lifetime: Lifetime) (directory: string): Task<Sprites> = task {
        let! brickResources = NeExeFile.LoadResources(Path.Combine(directory, "U95_BRIC.DLL"))
        let! backgrounds = Background.LoadBackgrounds(directory)
        let! titleScreen = Background.LoadBackground(Path.Combine(directory, "U95_T.SCR"))
        let! fishes = NeExeFile.LoadResources(Path.Combine(directory, "U95_PIC.DLL"))
        let! exeSprites = NeExeFile.LoadResources(Path.Combine(directory, "U95.EXE"))
        return {
            Bricks = loadBricks lifetime brickResources
            Background = loadBackgrounds lifetime backgrounds
            TitleScreenBackground = CreateSprite lifetime titleScreen
            Fishes = loadFishes lifetime fishes
            Player = PlayerSprites.Load lifetime exeSprites
            HUD = loadHUD lifetime exeSprites
            Bonuses = loadBonuses lifetime exeSprites
        }
    }
