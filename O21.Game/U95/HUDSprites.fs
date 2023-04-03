namespace O21.Game

open O21.Resources

module HUDSprites =
    
    let private hud = [| 5; 25; 26; 27; 28; 29; 184; 185; 186; 192; 193; 194; 195; 196; 197; 198; 199; 200; 201; 224; 159;|]
    let Load(sprites: Dib[]): Dib[] =
       Array.init hud.Length (fun i ->
           sprites[hud[i]]
    )
