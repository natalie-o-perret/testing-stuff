[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step5

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


module private Database =
    let getUser (UserId id) (connection: #ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: #IPaymentClient) : PaymentId = client.Charge card amount

let private inject f valueThatNeedsDep =
    fun (deps: 'deps) ->
        let value = valueThatNeedsDep deps
        f value deps

// Better than Step 4: use inferred inheritance by adding a # to the front of the type annotations for each dependency.
//  F# infers the type signature of chargeUser to be UserId -> float -> ('deps -> unit) and
// it requires that 'deps inherit from both ISqlConnection and IPaymentProvider.
let chargeUser userId amount =
    Database.getUser userId
    |> inject (fun user -> PaymentProvider.chargeCard user.CreditCard amount)
    // |> inject (fun (PaymentId paymentId) ->
    //    let email = EmailBody $"Your payment id is {paymentId}"
    //    let address = EmailAddress "a.customer@example.com"
    //    Email.sendMail address email)
