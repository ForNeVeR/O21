module O21.Game.U95.Parser

open System.IO

type MapOfLevel =
    | Brick of int
    | Bomb // random: octopus or bomb
    | Bonus // random bonus
    | None
    
type Parser(directory) =
     member private this.readDatFile(level:int) (part:int): string[] = 
        let lines = File.ReadAllLines(Path.Combine(directory, $"U95_{level}_{part}.DAT")) 
        lines
       
     member private this.parseLine (line:string): MapOfLevel[] =
         let levelMap = Array.init line.Length (fun i ->
            let brick = 
                match line[i] with
                    | t when t>='1' && t <= '9' -> Brick(int line[i] - int '0')
                    | 'b' -> Bomb
                    | 'a' -> Bonus
                    | _ -> None
            brick
         )
         levelMap
                
     member private this.parseLevel (data:string[]): MapOfLevel[][] = 
         let level = Array.init data.Length (fun i ->
            let bricks = this.parseLine data[i]
            bricks
         )
         level

     member this.LoadLevel (level:int) (part:int): MapOfLevel[][] =
        let lines = this.readDatFile level part
        this.parseLevel lines 
