module O21.Resources.Sprites

open System
open System.IO

open O21.NE

let Load(path: string, outDir: string): unit =
    use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
    let neFile = NeFile.ReadFrom stream
    let resources = neFile.ReadResourceTable() |> Seq.toArray
    let mutable i = 0
    for resource in resources[1].Resources do
        let data = neFile.ReadResourceContent resource
        let fileContent =
            [|
                yield! "BM"B
                yield! BitConverter.GetBytes (data.Length + 14)
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes 0x76
                yield! neFile.ReadResourceContent resource
            |]
        File.WriteAllBytes(outDir + "/" + (string i) + ".bmp", fileContent)
        i <- i + 1
    ()
