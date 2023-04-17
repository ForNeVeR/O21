namespace O21.Resources

open System.Buffers.Binary

type RGB = (struct(byte * byte * byte))

/// Device-independent bitmap.
type Dib(dib: byte[]) =

    let headerSize = 40

    let width = BinaryPrimitives.ReadInt32LittleEndian dib[4..8]
    let height = BinaryPrimitives.ReadInt32LittleEndian dib[8..12]
    let colorDepth = BinaryPrimitives.ReadUInt16LittleEndian dib[14..16]
    let paletteColorNumber =
        let number = BinaryPrimitives.ReadInt32LittleEndian dib[32..36]
        if number <> 0
        then number
        else 1 <<< int colorDepth

    let palette = Array.init paletteColorNumber (fun i ->
        let b = dib[headerSize + i * 4]
        let g = dib[headerSize + i * 4 + 1]
        let r = dib[headerSize + i * 4 + 2]
        struct(r, g, b)
    )

    member val Raw = dib
    member val Width = width
    member val Height = height
    member val Palette = palette
    member val PaletteColorNumber = paletteColorNumber
                                             
    member _.GetPixel(x: int, y: int): RGB =
        let y = height - y - 1 // turn the image upside-down
        if colorDepth = 1us then
            let mutable stride = width / 8
            if stride % 8 <> 0 then stride <- stride + (4 - stride % 4)
            let rowOffset = y * stride
            let byteIndex = rowOffset + x / 8
            let byteValue = dib[headerSize + paletteColorNumber * 4 + byteIndex]
            let paletteIndex = (byteValue >>> (7 - x % 8)) &&& byte 0x01
            palette[int paletteIndex]
        elif colorDepth = 4us then
            let mutable stride =  if width % 2 = 0 then width / 2 else width / 2 + 1
            if stride % 4 <> 0 then stride <- stride + (4 - stride % 4)
            let rowOffset = y * stride
            let byteIndex = rowOffset + x / 2
            let byteValue = dib[headerSize + paletteColorNumber * 4 + byteIndex]
            let paletteIndex =
                if x % 2 = 0
                then byteValue >>> 4
                else byteValue &&& 0b1111uy
            palette[int paletteIndex]
        elif colorDepth = 8us then
            let mutable stride = width 
            if stride % 4 <> 0 then stride <- stride + (4 - stride % 4) 
            let rowOffset = y * stride
            let byteIndex = rowOffset + x 
            let byteValue = dib[headerSize + paletteColorNumber * 4 + byteIndex]
            let paletteIndex = byteValue >>> 8
            palette[int paletteIndex]
        else failwith $"Unsupported color depth: {colorDepth}"
