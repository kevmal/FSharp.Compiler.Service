(*** hide ***)
#I "../../bin/"
(**
Compiler Services: Using the F# tokenizer
=========================================

Tokenizer is good!
*)

#r "FSharp.Compiler.Service.dll"
open Microsoft.FSharp.Compiler.SourceCodeServices

(**
Tokenizing
----------------------------------------------------------------------------
*)
let sourceTok = SourceTokenizer([], "C:\\test.fsx")

let tokenizeLines (lines:string[]) =
  [ let state = ref 0L
    for n, line in lines |> Seq.zip [ 0 .. lines.Length ] do
      let tokenizer = sourceTok.CreateLineTokenizer(line)
      let rec parseLine() = seq {
        match tokenizer.ScanToken(!state) with
        | Some(tok), nstate ->
            let str = line.Substring(tok.LeftColumn, tok.RightColumn - tok.LeftColumn + 1)
            yield str, tok
            state := nstate
            yield! parseLine()
        | None, nstate -> state := nstate }
      yield n, parseLine() |> List.ofSeq ]

let tokenizedLines = 
  tokenizeLines
    [| "// Sets the hello wrold variable"
       "let hello = \"Hello world\" " |]

for lineNo, lineToks in tokenizedLines do
  printfn "%d:  " lineNo
  for str, info in lineToks do printfn "       [%s:'%s']" info.TokenName str

(**
Fin!
*)