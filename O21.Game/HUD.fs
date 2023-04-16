namespace O21.Game

open Raylib_CsLo

type HUD() =

    let mutable score = 0
    let mutable level = 1
    let mutable oxy = 0f
    let mutable lives = 0
    let mutable abilities = Array.init 5 (fun _ -> false)

    member _.UpdateScore(newScore: int) =
        score <- newScore
        HUDRenderer.renderScoreLine score

    member _.UpdateOxy(newOxy: float32) =
        oxy <- newOxy
        HUDRenderer.renderOxyLine oxy

    member _.UpdateLives(newLives: int) =
        lives <- newLives
        HUDRenderer.renderLevel lives

    member _.UpdateLevel(newLevel: int) =
        level <- newLevel
        HUDRenderer.renderLevel level

    member _.Init(sprites: Texture[]) = HUDRenderer.renderAll sprites
