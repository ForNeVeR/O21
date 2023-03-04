module O21.Resources.Sprites

open System
open System.Buffers.Binary
open System.IO

open O21.NE

/// Device-independent bitmap.
type Dib = Dib of byte[]

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
    for Dib(dib) in images do
        let bitDepth = BinaryPrimitives.ReadInt16LittleEndian dib[14..16]
        let mutable paletteColors = BinaryPrimitives.ReadInt32LittleEndian dib[32..36]
        if paletteColors = 0 then paletteColors <- 1 <<< int bitDepth
        let pixelDataOffset =
            14 + // BMP header
            40 + // DIB header
            paletteColors * 4 // palette
        let fileContent =
            [|
                yield! "BM"B
                yield! BitConverter.GetBytes (dib.Length + 14)
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes 0us
                yield! BitConverter.GetBytes pixelDataOffset
                yield! dib
            |]
        File.WriteAllBytes(outDir + "/" + (string i) + ".bmp", fileContent)
        i <- i + 1
