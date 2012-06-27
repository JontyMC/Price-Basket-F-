open PriceBasket
open System

let productIds =
    System.Environment.GetCommandLineArgs()
    |> Seq.skip 1
    |> Seq.toList

totalBasket productIds DateTimeOffset.UtcNow