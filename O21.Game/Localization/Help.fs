module O21.Localization.Help

open O21.Game.Documents
open System.Collections.Generic
open System.Threading.Tasks
open System.IO
open System.Threading

type HelpRequest =
    | MarkdownHelp of string * CancellationToken
    | RussianHelp of (unit -> Task<DocumentFragment array>)

let private readMarkdown(file: string, cancellationToken: CancellationToken) =
    task {
        use sr = new StreamReader (file)
        let fragments = List<DocumentFragment> 0
        while not sr.EndOfStream do
            let! line = sr.ReadLineAsync cancellationToken
            fragments.Add (match line with
                            | "---" -> (DocumentFragment.Text(Style.Normal, "____________________________________________"))
                            | str when (str.Length > 0 && str[0] = '#') -> DocumentFragment.Text(Style.Bold, str.Substring(1))
                            | str -> (DocumentFragment.Text(Style.Normal, str)))
            fragments.Add (DocumentFragment.NewParagraph)
        return fragments.ToArray()
    }

let HelpDescription(request: HelpRequest) =
        match request with
            | MarkdownHelp (name, cancellationToken) -> readMarkdown($"Localization/Help/{name}.md", cancellationToken)
            | RussianHelp hlpReader -> hlpReader()
