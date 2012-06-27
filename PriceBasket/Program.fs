open PriceBasket

let productIds =
    System.Environment.GetCommandLineArgs()
    |> Seq.skip 1
    |> Seq.toList

totalBasket productIds