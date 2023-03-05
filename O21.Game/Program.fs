open O21.Game
open O21.Resources

[<EntryPoint>]
let main(args: string[]): int =
    match args with
    | [| "export"; inputFile; outDir |] ->
        Graphics.Load inputFile
        |> Graphics.Export outDir
    | [| dataDir |] ->
        let config = { 
            Title = "O21"
            GameWidth = 600
            GameHeight = 400
            ScreenWidth = 1200
            ScreenHeight = 800
            IsFullscreen = false
            IsFixedTimeStep = false
        }
        
        use loop =
            O21Game.game dataDir
            |> GameState.create config
            |> GameState.run

        ()
    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"

    0