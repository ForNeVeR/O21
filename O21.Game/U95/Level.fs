// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.U95

open System.IO
open System.Threading.Tasks

open O21.Game.U95.Parser

[<Struct>]
type LevelCoordinates =
    | LevelCoordinates of x: int * y: int
    member this.X = let (LevelCoordinates(x, _)) = this in x
    member this.Y = let (LevelCoordinates(_, y)) = this in y
    
type LevelEntitiesCoordinates = {
    Bombs: (int * int)[]
    Bonuses: (int * int)[]
}

type Level = {
    LevelMap: MapOfLevel[][]
    Position: int
    Coordinates: LevelCoordinates
}
    with
        static member Empty = {
            LevelMap = Array.empty
            Position = 0
            Coordinates = LevelCoordinates(0, 0)
        }
        
        static member private Load(directory: string) (level: int) (part: int): Task<Level> = task{
            let parser = Parser(directory)
            let coordinates = LevelCoordinates(part, level)
            let pos, level = parser.LoadLevel level part
            return {
                LevelMap = level
                Position = pos
                Coordinates = coordinates
            }
        }

        static member LoadAll(directory: string): Task<Map<LevelCoordinates, Level>> = task {
            do! Task.Yield()

            let! levelPairs =
                Directory.EnumerateFiles(directory, "U95_*.DAT")
                |> Seq.map(fun filePath -> task {
                    let levelCoordinates =
                        filePath
                        |> (nonNull << Path.GetFileNameWithoutExtension)
                        |> _.Split('_', 3)
                        |> Array.skip 1
                        |> Array.map int
                        |> function | [| y; x |] -> x, y
                                    | other -> failwith $"Impossible level name parts: %A{other}"
                        |> LevelCoordinates
                    let! level = Level.Load directory levelCoordinates.Y levelCoordinates.X
                    return levelCoordinates, level
                })
                |> Task.WhenAll
            return Map.ofArray levelPairs
        }
        
        member private this.EntitiesCoordinatesLazy = lazy (
                let bombCoords = ResizeArray()
                let bonusCoords = ResizeArray()
                this.LevelMap
                |> Array.iteri (fun i row ->
                    row
                    |> Array.iteri (fun j e ->
                        match e with
                        | Bomb -> bombCoords.Add(j, i)
                        | Bonus -> bonusCoords.Add(j, i)
                        | _ -> ()
                        ))
                {
                    Bombs = bombCoords |> _.ToArray()
                    Bonuses = bonusCoords |> _.ToArray()
                }
            )

        member this.BombsCoordinates() = this.EntitiesCoordinatesLazy.Value.Bombs
        
        member this.StaticBonusesCoordinates() = this.EntitiesCoordinatesLazy.Value.Bonuses
