module PriceBasketAcceptanceTests

open System
open System.IO
open System.Text.RegularExpressions
open FsUnit.Xunit
open Xunit
open Xunit.Extensions
open PriceBasket

let getPriceBasketOutput productIds =
    let fakeConsole = new StringWriter();

    Console.SetOut(fakeConsole)
    totalBasket productIds

    fakeConsole.ToString()

let matchBasketOutput productIds expected =
    let output = getPriceBasketOutput productIds

    output
    |> should equal expected

[<Fact>]
let ``No products in basket``() =
    matchBasketOutput [] "Subtotal: £0.00\r\n(No offers available)\r\nTotal: £0.00\r\n"

[<Fact>]
let ``Products in basket``() =
    matchBasketOutput ["Milk"; "Soup"; "Bread"; "Milk"] "Subtotal: £4.05\r\n(No offers available)\r\nTotal: £4.05\r\n"

[<Fact>]
let ``Half price offer applies``() =
    matchBasketOutput ["Soup"; "Soup"; "Bread"; "Milk"] "Subtotal: £3.40\r\nBuy 2 Soup get Bread half price: -£0.40\r\nTotal: £3.00\r\n"

[<Fact>]
let ``Not enough items for half price offer to apply``() =
    matchBasketOutput ["Soup"; "Bread"; "Milk"] "Subtotal: £2.75\r\n(No offers available)\r\nTotal: £2.75\r\n"

[<Fact>]
let ``Percentage discount applies``() =
    matchBasketOutput ["Apples"; "Bread"; "Milk"] "Subtotal: £3.10\r\nApples 10% off: -£0.10\r\nTotal: £3.00\r\n"

[<Fact>]
let ``Percentage discount scheduled in future``() =
    matchBasketOutput ["Apples"; "Bread"; "Milk"] "Subtotal: £3.10\r\nApples 10% off: -£0.10\r\nTotal: £3.00\r\n"