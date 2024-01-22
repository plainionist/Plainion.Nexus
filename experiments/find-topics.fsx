
open System.IO
open System.Text.RegularExpressions

let getWords (text:string) =
  Regex.Matches(text, @"[a-zA-Z0-9-_]+")
  |> Seq.cast<Match>
  |> Seq.map(fun x -> x.Value)

let isWikiWord (word:string) = Regex.IsMatch(word, "^([A-Z][a-z0-9]+){2,}$")
let isTag (word:string) = word.StartsWith("#")
let isCombinedWord (word:string) = word.Contains('-')
let isAbbreviation (word:string) = Regex.IsMatch(word, "^[A-Z]{3,}$") && word.Equals("TODO") |> not

let (||.) f g x = f x || g x
let isTopic = isCombinedWord ||. isTag ||. isWikiWord ||. isAbbreviation

printfn "Topics:"

Directory.GetFiles(fsi.CommandLineArgs[1], "*.md", SearchOption.AllDirectories)
|> Seq.map File.ReadAllText
|> Seq.collect getWords
|> Seq.filter isTopic
|> Seq.distinct
|> Seq.iter (printfn "  %s")