[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step3

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


module private Database =

    let getUser (UserId id) (connection: ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: IPaymentClient) = client.Charge card amount


// Slightly better than Step 2: we've managed to defer the application of any dependencies,
// but the solution is a bit cumbersome still.
// If, at a later date, we need to do more computations in chargeUser that require yet more dependencies,
// then we're going to be faced with even more lambda writing.

let chargeUser userId amount : (ISqlConnection -> IPaymentClient -> PaymentId) =
    let userFromConnection = Database.getUser userId

    fun connection ->
        let user = userFromConnection connection
        PaymentProvider.chargeCard user.CreditCard amount

// For instance imagine we wanted to email the user a receipt with the PaymentId.
// Then we'd have to write something like this:

// let paymentIdFromClient = PaymentProvider.chargeCard user.CreditCard amount
// fun paymentClient ->
// let (PaymentId paymentId) = paymentIdFromClient paymentClient
// let email = EmailBody $"Your payment id is {paymentId}"
// Email.sendMail user.EmailAddress email
