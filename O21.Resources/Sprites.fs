module O21.Resources.Sprites

open System.IO

open O21.NE

let Load(path: string) =
    use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
    let neFile = NeFile.ReadFrom stream
    let resources = neFile.ReadResourceTable() |> Seq.toArray
    ()
