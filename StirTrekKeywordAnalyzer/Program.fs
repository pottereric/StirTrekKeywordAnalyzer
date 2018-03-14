open canopy
open Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models

let azureRegion = AzureRegions.Westcentralus
let subscriptionKey =  "<Your-Subscription-Key>"


let analyzeStirTrekTags _ =
    start Chrome
    //pin FullScreen
    url "https://www.stirtrek.com/Schedule?flat=False&year=2018"
    let titles = elements ".sessionTitle"
    //titles |> List.iter (fun t -> printfn "%s" (read t))

    let inputs = titles |> List.mapi(fun i t -> ("en", i.ToString(), (read t)))

    let client = FognitiveServices.Text.Client.create subscriptionKey azureRegion
    let results = FognitiveServices.Text.Client.getTags client inputs
    
    results.Documents |> Seq.iter (fun r -> r.KeyPhrases |> Seq.iter (fun kp -> printfn "%s %s" r.Id kp))

    ()

[<EntryPoint>]
let main argv = 
    analyzeStirTrekTags()
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
