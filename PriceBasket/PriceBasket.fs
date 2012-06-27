module PriceBasket

open System

type Product = { Id : string; Price : decimal }

type SpecialOffer =
    abstract member IsValid : unit -> bool
    abstract member DisplayText : unit -> string
    abstract member CalculateDiscount : seq<Product> -> decimal

type AppliedSpecialOffer = { DisplayText1 : string; Discount : decimal } 

let availableProducts = [
    { Id = "Soup"; Price = 0.65m }
    { Id = "Bread"; Price = 0.8m }
    { Id = "Milk"; Price = 1.3m }
    { Id = "Apples"; Price = 1m }
]

let productById id =
    availableProducts
    |> Seq.find (fun y -> y.Id = id)

let getSelectedProducts productIds =
    productIds
    |> Seq.map productById

let scheduledPercentageOffer qualifyingProduct percentageDiscount startDate endDate =
    { new SpecialOffer with
        member this.IsValid() =
            let isBetweenTimes startDate endDate =
                let now = System.DateTimeOffset.UtcNow

                startDate < now && now < endDate

            isBetweenTimes startDate endDate

        member this.DisplayText() = sprintf "%s %s off" qualifyingProduct.Id (percentageDiscount.ToString())

        member this.CalculateDiscount(products) =
            let individualDiscount product = product.Price * percentageDiscount / 100m

            products
            |> Seq.filter (fun x -> x = qualifyingProduct)
            |> Seq.sumBy individualDiscount
    }

let halfPriceOffer qualifyingProduct quantityToQualify discountProduct =
    { new SpecialOffer with
        member this.IsValid() = true

        member this.DisplayText() = sprintf "Buy %d %s get %s half price" quantityToQualify qualifyingProduct.Id discountProduct.Id

        member this.CalculateDiscount(products) =
            let qualifyingProductCount =
                products
                |> Seq.filter (fun x -> x = qualifyingProduct)
                |> Seq.length

            let qualifyingCount = qualifyingProductCount / quantityToQualify

            let discountProductCount =
                products
                |> Seq.filter (fun x -> x = discountProduct)
                |> Seq.take qualifyingCount
                |> Seq.length

            (decimal)qualifyingCount * discountProduct.Price / 2m
    }

let availableOffers = [
    scheduledPercentageOffer (productById "Apples") 10m (DateTimeOffset(2012, 1, 20, 0, 0, 0, TimeSpan.Zero)) (DateTimeOffset(2013, 1, 27, 0, 0, 0, TimeSpan.Zero))
    halfPriceOffer (productById "Soup") 2 (productById "Bread")
]

let subtotal selectedProducts =
    selectedProducts
    |> Seq.sumBy (fun x -> x.Price)

let applyOffers availableOffers selectedProducts =
    let applyOffer(offer: SpecialOffer) = { DisplayText1 = offer.DisplayText(); Discount = offer.CalculateDiscount(selectedProducts) }

    let filterValidOffer(offer: SpecialOffer) = offer.IsValid()

    availableOffers
    |> Seq.filter filterValidOffer
    |> Seq.map applyOffer
    |> Seq.filter (fun x -> x.Discount > 0m)

let total subtotal appliedOffers = 
    let totalDiscount =
        appliedOffers
        |> Seq.sumBy (fun x -> x.Discount)

    subtotal - totalDiscount

let formatPrice price =
    System.String.Format("£{0:0.00}", (decimal)price)

let renderTotal subtotal appliedOffers total =
    let displayOffer appliedOffer = printfn "%s: -%s" appliedOffer.DisplayText1 (formatPrice appliedOffer.Discount)
        
    printfn "Subtotal: %s" (formatPrice subtotal)
    if appliedOffers |> Seq.length = 0
    then printfn "(No offers available)"
    else
        appliedOffers
        |> Seq.iter displayOffer
    printfn "Total: %s" (formatPrice total)

let totalBasket productIds =
    let selectedProducts = getSelectedProducts productIds

    let subtotal = subtotal selectedProducts

    let appliedOffers = applyOffers availableOffers selectedProducts

    let total = total subtotal appliedOffers

    renderTotal subtotal appliedOffers total