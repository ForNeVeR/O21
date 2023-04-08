module O21.Game.U95.Help

open System.IO
open System.Text
open System.Threading.Tasks

open Microsoft.Xna.Framework.Graphics
open O21.Game.Documents
open O21.MRB
open O21.Resources
open O21.WinHelp
open O21.WinHelp.Fonts
open O21.WinHelp.Topics
open Oxage.Wmf.Records

let private loadFontDescriptors(content: byte[]) =
    use stream = new MemoryStream(content)
    let fontFile = FontFile.Load(stream)
    fontFile.ReadDescriptors()

let private style = function
    | None -> Style.Normal
    | Some(font: FontDescriptor) ->
        match font.Attributes with
        | FontAttributes.Bold -> Style.Bold
        | FontAttributes.Normal -> Style.Normal
        | _ -> failwith $"Unknown font attributes: {font.Attributes}"

let private convertParagraphs (fonts: FontDescriptor[]) (bitmaps: int -> Texture2D) (items: IParagraphItem seq) = seq {
    let mutable currentFont = None
    for item in items do
        match item with
        | :? FontChange as fc ->
            let font = fonts[int fc.FontDescriptor]
            currentFont <- Some font
        | :? ParagraphText as pc ->
            yield DocumentFragment.Text(style currentFont, pc.Text)
        | :? NewParagraph ->
            yield DocumentFragment.NewParagraph
        | :? Bitmap as b ->
            yield DocumentFragment.Image(bitmaps <| int b.Number)
        | _ -> failwith $"Unknown paragraph item: {item}"
}

let extractDibImageFromMrb(file: byte[]): Dib =
    use stream = new MemoryStream(file)
    let file = MrbFile.Load stream
    if file.ImageCount <> 1s then
        failwith "Invalid image count."

    let image = file.ReadImage 0

    let document = file.ReadWmfDocument image
    let record =
        document.Records
        |> Seq.filter (fun x -> x :? WmfStretchDIBRecord)
        |> Seq.exactlyOne
        :?> WmfStretchDIBRecord

    use stream = new MemoryStream()
    use writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen = true)
    record.DIB.Write writer

    Dib <| stream.ToArray()

let private loadTopic encoding fonts bitmaps (content: byte[]) =
    use stream = new MemoryStream(content)
    TopicFile.Load(stream).ReadParagraphs()
    |> Seq.filter(fun p -> p.RecordType = ParagraphRecordType.TextRecord)
    |> Seq.collect(fun p -> p.ReadItems(encoding).Items)
    |> convertParagraphs fonts bitmaps
    |> Seq.toArray

let Load (gd: GraphicsDevice) (helpFile: string): Task<DocumentFragment[]> = task {
    use input = new FileStream(helpFile, FileMode.Open, FileAccess.Read)
    let helpFile = WinHelpFile.Load input
    let files =
        helpFile.GetFiles Encoding.UTF8
        |> Seq.map(fun d -> d.FileName, d)
        |> Map.ofSeq

    let fonts =
        helpFile.ReadFile files["|FONT"]
        |> loadFontDescriptors

    let bitmaps index =
        let name = $"|bm{string index}"
        let file = files[name]
        let dib = extractDibImageFromMrb <| helpFile.ReadFile file
        Sprites.CreateSprite gd dib

    let contentEncoding = Encoding.GetEncoding 1251 // TODO: Extract from config
    return
        helpFile.ReadFile files["|TOPIC"]
        |> loadTopic contentEncoding fonts bitmaps
        |> Seq.toArray
}
