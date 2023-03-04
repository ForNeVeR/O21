module O21.Game.Program

open O21.Resources

[<EntryPoint>]
let main(args: string[]): int =
    Sprites.Load(args[0])
    use game = new O21Game()
    game.Run()
    0
