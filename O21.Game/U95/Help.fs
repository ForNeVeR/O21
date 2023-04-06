module O21.Game.U95.Help

open System.IO
open System.Text
open System.Threading.Tasks

open O21.Game.Documents
open O21.WinHelp
open O21.WinHelp.Fonts
open O21.WinHelp.Topics

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

let private convertParagraphs (fonts: FontDescriptor[]) (items: IParagraphItem seq) = seq {
    let mutable currentFont = None
    for item in items do
        match item with
        | :? FontChange as fc ->
            let font = fonts[int fc.FontDescriptor]
            if Option.isSome currentFont then failwith "Attempt to reset font when font is already set."
            currentFont <- Some font
        | :? ParagraphText as pc ->
            yield DocumentFragment.Text(style currentFont, pc.Text)
            currentFont <- None
        | _ -> failwith $"Unknown paragraph item: {item}"
}

let private loadTopic encoding fonts (content: byte[]) =
    use stream = new MemoryStream(content)
    TopicFile.Load(stream).ReadParagraphs()
    |> Seq.filter(fun p -> p.RecordType = ParagraphRecordType.TextRecord)
    |> Seq.collect(fun p -> p.ReadItems(encoding).Items)
    |> convertParagraphs fonts
    |> Seq.toArray

let Load(helpFile: string): Task<DocumentFragment[]> = task {
    use input = new FileStream(helpFile, FileMode.Open, FileAccess.Read)
    let helpFile = WinHelpFile.Load input
    let files =
        helpFile.GetFiles Encoding.UTF8
        |> Seq.map(fun d -> d.FileName, d)
        |> Map.ofSeq

    let fonts =
        helpFile.ReadFile files["|FONT"]
        |> loadFontDescriptors

    let contentEncoding = Encoding.GetEncoding 1251 // TODO: Extract from config
    return
        helpFile.ReadFile files["|TOPIC"]
        |> loadTopic contentEncoding fonts
        |> Seq.toArray
}
