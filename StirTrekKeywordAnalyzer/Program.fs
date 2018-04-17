open canopy

let getKeyPhrasesFromResult (result : Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models.KeyPhraseBatchResult) =
    result.Documents
    |> Seq.collect (fun d -> (d.KeyPhrases))
    |> Seq.map (fun p -> p.ToLower())

let getKeyPhrasesForYear year = 
    start Chrome
    url ("https://www.stirtrek.com/Schedule?flat=False&year=" + year)

    let client = FognitiveServices.Text.Client.create Config.subscriptionKey Config.azureRegion

    let sessionTitlesKeywords = 
        elements ".sessionTitle" 
        |> List.map (fun t -> read t)
        |> List.mapi(fun i t -> ("en", i.ToString(), t))
        |> FognitiveServices.Text.Client.getTags client 
        |> getKeyPhrasesFromResult
    
    click ".showMoreDetails"
    let absractKeywords = 
        elements ".sessionTitle + div"
        |> List.map(fun e -> (read e).Replace("[Show Less]", ""))
        |> List.mapi(fun i t -> ("en", i.ToString(), t))
        |> FognitiveServices.Text.Client.getTags client 
        |> getKeyPhrasesFromResult

    sessionTitlesKeywords, absractKeywords
    
let getTitleKeyPhrasesForYear year =
    let (titles,_) = getKeyPhrasesForYear year
    titles

let getAbstractsKeyPhrasesForYear year =
    let (_, abstracts) = getKeyPhrasesForYear year
    abstracts

type phraseCount = {
    Phrase : string
    Count : int}

let count_dup l =
  let scan_count x l = List.fold(fun (c,acc) y -> if x = y then c+1,acc else c,y::acc) (1,[]) l in
  let rec count acc = function
    | [] -> List.rev acc
    | hd::tl -> let c,r = scan_count hd tl in count ((hd,c)::acc) r
  in 
  count [] l

let showUnionOfAllTitleTags phrasesA phrasesB =
    let phraseSetA = phrasesA |> Set.ofSeq
    let phraseSetB = phrasesB |> Set.ofSeq

    let intersection = Set.intersect phraseSetA phraseSetB
    intersection |> Set.iter (fun kp -> printfn "%s" kp)


let showCountsOfAllAbstractTags phrasesA phrasesB  =
    let phraseSetA = phrasesA |> List.ofSeq 
    let phraseSetB = phrasesB |> List.ofSeq 

    let allPhrases = List.append phraseSetA phraseSetB
    let results = allPhrases 
                    |> count_dup
                    |> List.map(fun i -> 
                        let phrase, count = i
                        {Phrase = phrase; Count = count})

    results 
    //|> List.where(fun r -> r.Count > 2)
    |> List.sortByDescending(fun r -> r.Count)
    |> List.iter(fun r ->
        printfn "%s %d" r.Phrase r.Count
    )

let testGetAbstract _ =
    start Chrome
    url ("https://www.stirtrek.com/Schedule?flat=False&year=2017")
    click ".showMoreDetails"

    let absracts = 
        elements ".sessionTitle + div"
        |> List.map(fun e -> (read e).ToLower().Replace("[show less]", ""))
    absracts |> List.iter (fun a -> printfn "%s" a)


[<EntryPoint>]
let main argv = 

    let phrases2018 = getKeyPhrasesForYear "2018"
    let phrases2017 = getKeyPhrasesForYear "2017"

    showUnionOfAllTitleTags (fst phrases2017) (fst phrases2018)
    showCountsOfAllAbstractTags (snd phrases2017) (snd phrases2018)
    //testGetAbstract()

    printfn "Press Any Key"
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
