module O21.Game.U95.NeExeFile

open System.IO
open System.Text
open System.Threading.Tasks

open Oddities.NE
open Oddities.Resources

let LoadResources(path: string): Task<Dib[]> = task {
    do! Task.Yield()
    
    use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
    use reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)
    let neFile = NeFile.ReadFrom reader
    let resources = neFile.ReadResourceTable()
    let bitmapResourceType =
        resources
        |> Seq.filter (fun x -> x.TypeId = 32770us)
        |> Seq.exactlyOne
    return
        bitmapResourceType.Resources
        |> Seq.map neFile.ReadResourceContent
        |> Seq.map Dib
        |> Seq.toArray
}
