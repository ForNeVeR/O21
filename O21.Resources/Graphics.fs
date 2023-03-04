module O21.Resources.Graphics

open System
open System.IO

open O21.NE

let Load(path: string): Dib[] =
    use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
    let neFile = NeFile.ReadFrom stream
    let resources = neFile.ReadResourceTable()
    let bitmapResourceType =
        resources
        |> Seq.filter (fun x -> x.TypeId = 32770us)
        |> Seq.exactlyOne
    bitmapResourceType.Resources
    |> Seq.map neFile.ReadResourceContent
    |> Seq.map Dib
    |> Seq.toArray

let Export (outDir: string) (images: Dib seq): unit =
    Directory.CreateDirectory outDir |> ignore

    let mutable i = 0
    for dib in images do
        let pixelDataOffset =
            14 + // BMP header
            40 + // DIB header
            dib.Palette.Length * 4 // palette
        let fileContent =
            [|
                yield! "BM"B
                yield! BitConverter.GetBytes(dib.Raw.Length + 14)
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes pixelDataOffset
                yield! dib.Raw
            |]
        File.WriteAllBytes(outDir + "/" + (string i) + ".bmp", fileContent)
        i <- i + 1
