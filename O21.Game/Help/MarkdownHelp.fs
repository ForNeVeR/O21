module O21.Game.Help.MarkdownHelp

open System
open System.IO

open FSharp.Formatting.Markdown
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game

let private loadTextureFromData (readFile: string -> byte[]) (address: string): Texture =
    let fileName = $"|{address}"
    readFile fileName
    |> HlpFile.ExtractDibImageFromMrb
    |> TextureUtils.CreateSprite

let private parseParagraph readFile paragraph =
    let parseSpans(spans, isHeading) =
        let getImage(link: Uri) =
            match link.Scheme with
            | "data" -> DocumentFragment.Image <| loadTextureFromData readFile link.Authority
            | _ -> failwith $"Unsupported image URI in help file: {link}"

        seq {
            let style = if isHeading then Style.Bold else Style.Normal
            for span in spans do
                match span with
                | Literal (text = text) -> yield DocumentFragment.Text(style, text.Replace("\r\n", "\n"))
                | DirectImage(link = link) -> yield getImage(Uri link)
                | _ -> ()
            yield DocumentFragment.NewParagraph
        }

    match paragraph with
        | Heading (body = body) -> parseSpans(body, true)
        | Paragraph (body = body) -> parseSpans(body, false)
        | HorizontalRule _ -> [| DocumentFragment.Text(Style.Normal, "____________________________________________"); DocumentFragment.NewParagraph  |]
        | _ -> [||]

let Load (hlpFilePath: string) (markdownFilePath: string): DocumentFragment[] =
    use hlpFileStream = new FileStream(hlpFilePath, FileMode.Open, FileAccess.Read)
    let hlpFile, hfs = HlpFile.ReadMainData hlpFileStream
    let readFile name =
        hfs[name] |> hlpFile.ReadFile

    // TODO[#93]: Make this async
    let markdown = File.ReadAllText markdownFilePath
    let fragments = ResizeArray<DocumentFragment> 0
    let parsed = Markdown.Parse(markdown)
    for paragraph in parsed.Paragraphs do
            paragraph |> parseParagraph readFile |> fragments.AddRange
    fragments.ToArray()
