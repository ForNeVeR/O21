module O21.Game.U95.Parser

open System.Collections.Generic
open System.IO
open Microsoft.Xna.Framework.Graphics

module Parser =
     let private ReadDatFile (directory:string) (level:int) (part:int) : string[] = 
        try 
            let lines = File.ReadAllLines(Path.Combine(directory, $"U95_{level}_{part}.DAT")) 
            lines
        with
            | Exception -> [|"Parser can`t find file"|]
        
     let private ParseLine (line:string) (bricks:ICollection<Texture2D>) =
         let listOfBricks = bricks |> List
         let levelMap = List<Option<Texture2D>>()
         for character in line do
             match character with
                | t when t>='1' && t <= '9' -> levelMap.Add(Some(listOfBricks[int character - int '0' - 1]))
                | _ -> levelMap.Add(None)
                // need to add more cases when other textures will be available
         levelMap
                
     let private ParseLevel (data:string[]) (bricks:ICollection<Texture2D>): List<List<Option<Texture2D>>> = 
         let level = List<List<Option<Texture2D>>>()
         for line in data do
             level.Add (ParseLine line bricks)
         level

     let LoadLevel (directory:string) (bricks:ICollection<Texture2D>) (level:int) (part:int) =
        let lines = ReadDatFile directory level part
        ParseLevel lines bricks 
