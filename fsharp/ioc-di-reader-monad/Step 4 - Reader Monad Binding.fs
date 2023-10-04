[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step4

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


module private Database =

    let getUser (UserId id) (connection: ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: IPaymentClient) = client.Charge card amount


let private inject f valueThatNeedsDep =
    fun (deps: 'deps) ->
        let value = valueThatNeedsDep deps
        f value deps

// Slightly better than Step 3: inject that will allow us to simplify chargeUser
// by removing the need for us to write the lambda that supplies the ISqlConnection.
// - It works for any function f that needs a value a which can be created when passed some dependency.
// - The inject function is letting us sequence computations that each depend on
// a wrapped value returned from the last computation.
// - It turns out that we've in fact discovered bind again, but this time for a new monad.
// - This new monad is normally called Reader because it can be thought of
// as reading some value from an environment.
let chargeUser userId amount =
    Database.getUser userId
    |> inject (fun user -> PaymentProvider.chargeCard user.CreditCard amount)
    // |> inject (fun (PaymentId paymentId) ->
    //    let email = EmailBody $"Your payment id is {paymentId}"
    //    let address = EmailAddress "a.customer@example.com"
    //    Email.sendMail address email)
