open canopy

let analyzeStirTrekTags _ =
    start Chrome
    pin FullScreen
    url "https://www.stirtrek.com/Schedule?flat=False&year=2018"
    let titles = elements ".sessionTitle"
    titles |> List.iter (fun t -> printfn "%s" (read t))
    ()

[<EntryPoint>]
let main argv = 
    analyzeStirTrekTags()
    System.Console.ReadKey() |> ignore
    0 // return an integer exit code
