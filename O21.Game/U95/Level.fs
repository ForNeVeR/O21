// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
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

type Level = {
    LevelMap: MapOfLevel[][]
}
    with 
        static member private Load(directory:string) (level:int) (part:int): Task<Level> = task{
            let parser = Parser(directory)
            let level = parser.LoadLevel level part
            return{
                LevelMap = level;
            }
        }

        static member LoadAll(directory: string): Task<Map<LevelCoordinates, Level>> = task {
            do! Task.Yield()

            let! levelPairs =
                Directory.EnumerateFiles(directory, "U95_*.DAT")
                |> Seq.map(fun filePath -> task {
                    let levelCoordinates =
                        filePath
                        |> Path.GetFileNameWithoutExtension
                        |> fun s -> s.Split('_', 3)
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
