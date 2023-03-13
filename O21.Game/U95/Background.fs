namespace O21.Game.U95

open System.IO
open O21.Resources

module Background =
    
    // this parts represent background for levels
    // 'T' - splash screen
    // '1'..'9' - levels
    // 'W' - background for upper levels 
    // 'G' - background for lower levels
    let private parts = ['T'; '1'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9'; 'W'; 'G';] 
    
    let LoadBackground (path:string): Dib =
        let bytes = File.ReadAllBytes path 
        bytes
        |> Array.skip 14 // skip bitmap file header bytes
        |> Dib
        
    let LoadBackgrounds (directory:string): Dib[] =
        Array.init parts.Length (fun i ->
            LoadBackground (Path.Combine(directory, $"U95_{parts[i]}.SCR"))  
        )
