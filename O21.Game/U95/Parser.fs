module O21.Game.U95.Parser

open System.Collections.Generic
open System.IO
open Microsoft.Xna.Framework.Graphics

type Parser(directory, textures:ICollection<Texture2D>) =
     member private this.ReadDatFile(level:int) (part:int) : string[] = 
        try 
            let lines = File.ReadAllLines(Path.Combine(directory, $"U95_{level}_{part}.DAT")) 
            lines
        with
            | Exception -> [|"Parser can`t find file"|]
        
     member private this.ParseLine (line:string) =
         let listOfBricks = textures |> List
         let levelMap = List<Option<Texture2D>>()
         for character in line do
             match character with
                | t when t>='1' && t <= '9' -> levelMap.Add(Some(listOfBricks[int character - int '0' - 1]))
                | _ -> levelMap.Add(None)
                // need to add more cases when other textures will be available
         levelMap
                
     member private this.ParseLevel (data:string[]): List<List<Option<Texture2D>>> = 
         let level = List<List<Option<Texture2D>>>()
         for line in data do
             level.Add (this.ParseLine line)
         level

     member this.LoadLevel (level:int) (part:int) =
        let lines = this.ReadDatFile level part
        this.ParseLevel lines 

