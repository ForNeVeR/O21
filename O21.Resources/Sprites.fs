module O21.Resources.Sprites

open System
open System.Buffers.Binary
open System.IO

open O21.NE

let Load(path: string, outDir: string): unit =
    use stream = new FileStream(path, FileMode.Open, FileAccess.Read)
    let neFile = NeFile.ReadFrom stream
    let resources = neFile.ReadResourceTable() |> Seq.toArray
    let mutable i = 0
    for resource in resources[1].Resources do
        let dib = neFile.ReadResourceContent resource
        let bitness = BinaryPrimitives.ReadInt16LittleEndian dib[14..16]
        let mutable paletteColors = BinaryPrimitives.ReadInt32LittleEndian dib[32..36]
        if paletteColors = 0 then paletteColors <- 1 <<< int bitness
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
                yield! neFile.ReadResourceContent resource
            |]
        File.WriteAllBytes(outDir + "/" + (string i) + ".bmp", fileContent)
        i <- i + 1
    ()
