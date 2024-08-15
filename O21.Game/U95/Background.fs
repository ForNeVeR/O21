namespace O21.Game.U95

open System.IO
open System.Threading.Tasks

open Oddities.Resources

module Background =
    
    let private parts = [|'T'; '1'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9'; 'W'; 'G';|] 
    
    let LoadBackground(path:string): Task<Dib> = task {
        let! bytes = File.ReadAllBytesAsync path 
        return 
            bytes
            |> Array.skip 14 // skip bitmap file header bytes
            |> Dib
    }
        
    let LoadBackgrounds (directory:string): Task<Dib[]> =
        Array.init parts.Length (fun i ->
            LoadBackground (Path.Combine(directory, $"U95_{parts[i]}.SCR"))  
        ) |> Task.WhenAll
