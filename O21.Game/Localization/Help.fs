module O21.Localization.Help

open O21.Game.Documents
open System.Collections.Generic
open System.Threading.Tasks
open System.IO
open System.Threading
open LocalizationPaths
open FSharp.Formatting.Markdown
open System.Linq

type HelpRequest =
    | MarkdownHelp of string * CancellationToken
    | RussianHelp of (unit -> Task<DocumentFragment array>)
  
type private Width = uint
type private Height = uint
type private PngChunk = 
    | End
    | Ihdr of Width * Height

let private parseParagraph(paragraph) = 
    let rec parseSpans(spans, isHeading) =
        let getPng(link) : Option<Raylib_CsLo.Texture> =
            None

        let getImage(body, link, title) =
            let unhandledValue = 
               [| DocumentFragment.Text(Style.Normal, (match title with 
                                                        | Some title -> title
                                                        | None -> body) + " "); DocumentFragment.NewParagraph |]

            let imageFolder = HelpImagesFolder()
            let image = Directory.GetFiles(imageFolder, $"{link}.*").FirstOrDefault()
            let extension = Path.GetExtension(image)
            match (match extension with
                    | ".png" -> getPng(link)
                    | _  -> None) with
                | Some texture -> [| DocumentFragment.Image texture |]
                | None -> unhandledValue

        seq {
            let style = if isHeading then Style.Bold else Style.Normal
            for span in spans do
                match span with
                | Literal (text = text) -> yield DocumentFragment.Text(style, text.Replace("\r\n", "\n")); yield DocumentFragment.NewParagraph
                | DirectImage (body = body; link = link; title = title) -> yield! getImage(body, link, title)
                | _ -> ()
        }

    match paragraph with
        | Heading (body = body) -> parseSpans(body, true)
        | Paragraph (body = body) -> parseSpans(body, false)
        | HorizontalRule _ -> [| DocumentFragment.Text(Style.Normal, "____________________________________________"); DocumentFragment.NewParagraph  |]
        | _ -> [||]

let private readMarkdown(file: string, cancellationToken: CancellationToken) =
    task {
        use sr = new StreamReader (file)
        let! text = sr.ReadToEndAsync cancellationToken
        let fragments = List<DocumentFragment> 0
        let parsed = Markdown.Parse(text)
        for paragraph in parsed.Paragraphs do
                paragraph |> parseParagraph |> fragments.AddRange
        return fragments.ToArray()
    }

let HelpDescription(request: HelpRequest) =
        match request with
            | MarkdownHelp (name, cancellationToken) -> readMarkdown($"{HelpFolder()}/{name}.md", cancellationToken)
            | RussianHelp hlpReader -> hlpReader()
