// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.U95.Parser

open System.IO

type MapOfLevel =
    | Brick of int
    | Bomb // random: octopus or bomb
    | Bonus // random bonus
    | Empty
    
type Parser(directory) =
     member private this.getLevelPosition (part: int) =
         if part <= 4 then 1 else
            if part <= 9 then 2 else
                let d1, d2 = (part / 10, part % 10)
                let pos = 2 * d1 + 1
                if d2 < 5 then pos else pos + 1
         
     member private this.readDatFile(level:int) (part:int): string[] = 
        let lines = File.ReadAllLines(Path.Combine(directory, $"U95_{level}_{part}.DAT")) 
        lines
       
     member private this.parseLine (line:string): MapOfLevel[] =
         let levelMap = Array.init line.Length (fun i ->
            let brick = 
                match line[i] with
                    | t when t >='1' && t <= '9' -> Brick(int line[i] - int '0')
                    | 'b' -> Bomb
                    | 'a' -> Bonus
                    | _ -> Empty
            brick
         )
         levelMap
                
     member private this.parseLevel (data:string[]): MapOfLevel[][] = 
         let level = Array.init data.Length (fun i ->
            let bricks = this.parseLine data[i]
            bricks
         )
         level

     member this.LoadLevel (level:int) (part:int): int * MapOfLevel[][] =
        let pos = this.getLevelPosition part
        let lines = this.readDatFile level part
        (pos, this.parseLevel lines)
