open canopy

let analyzeStirTrekTags _ =
    start Chrome
    url "https://www.stirtrek.com/Schedule?flat=False&year=2018"

    let client = FognitiveServices.Text.Client.create Config.subscriptionKey Config.azureRegion

    let results = 
        elements ".sessionTitle" 
        |> List.mapi(fun i t -> ("en", i.ToString(), (read t)))
        |> FognitiveServices.Text.Client.getTags client 

    results.Documents |> Seq.iter (fun r -> r.KeyPhrases |> Seq.iter (fun kp -> printfn "%s %s" r.Id kp))

[<EntryPoint>]
let main argv = 
    analyzeStirTrekTags()
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
