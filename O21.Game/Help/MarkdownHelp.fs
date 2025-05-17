// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Help.MarkdownHelp

open System
open System.IO
open System.Text

open FSharp.Formatting.Markdown
open JetBrains.Lifetimes
open Raylib_CSharp
open type Raylib_CSharp.Raylib

open O21.Game
open Raylib_CSharp.Textures

let private LoadTextureFromData lifetime (readFile: string -> byte[]) (address: string): Texture2D =
    let fileName = $"|{address}"
    readFile fileName
    |> HlpFile.ExtractDibImageFromMrb
    |> TextureUtils.CreateSprite lifetime

let private parseParagraph lifetime readFile paragraph =
    let parseSpans(spans, isHeading) =
        let getImage(link: Uri) =
            match link.Scheme with
            | "data" -> DocumentFragment.Image <| LoadTextureFromData lifetime readFile link.Authority
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

let Load (lifetime: Lifetime) (hlpFilePath: string) (markdownFilePath: string): DocumentFragment[] =
    use hlpFileStream = new FileStream(hlpFilePath, FileMode.Open, FileAccess.Read)
    use reader = new BinaryReader(hlpFileStream, Encoding.UTF8, leaveOpen = true)
    let hlpFile, hfs = HlpFile.ReadMainData reader
    let readFile name =
        hfs[name] |> hlpFile.ReadFile

    // TODO[#93]: Make this async
    let markdown = File.ReadAllText markdownFilePath
    let fragments = ResizeArray<DocumentFragment> 0
    let parsed = Markdown.Parse(markdown)
    for paragraph in parsed.Paragraphs do
            paragraph |> parseParagraph lifetime readFile |> fragments.AddRange
    fragments.ToArray()
