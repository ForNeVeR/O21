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

let private convertParagraph fonts (item: IParagraphItem): IDocumentFragment =
    match item with
    | :? FontChange -> failwith "TODO: Process font change and apply to the next block"
    | _ -> failwith $"Unknown help paragraph item: {item}"

let private loadTopic encoding fonts (content: byte[]) =
    use stream = new MemoryStream(content)
    TopicFile.Load(stream).ReadParagraphs()
    |> Seq.filter(fun p -> p.RecordType = ParagraphRecordType.TextRecord)
    |> Seq.collect(fun p -> p.ReadItems(encoding).Items)
    |> Seq.map(convertParagraph fonts)
    |> Seq.toArray

let Load(helpFile: string): Task<IDocumentFragment[]> = task {
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
