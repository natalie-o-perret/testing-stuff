[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step1

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


module private Database =

    let getUser (UserId id) : User =
        let connection: ISqlConnection = SqlConnection("my-connection-string")
        connection.QueryUser($"SELECT * FROM User AS u WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount =
        let client: IPaymentClient = PaymentClient("my-payment-api-secret")
        client.Charge card amount


// Meh 😑 because: instantiating bts new clients like SqlConnection and PaymentClient which creates a few problems
// - Hard coded connection strings mean we're stuck talking to the same database instance in all environments.
// - Connection strings usually contain secrets which are now checked into source control.
// - Writing unit tests isn't possible because it's going to be calling the production database and payment provider.
let chargeUser userId amount =
    let user = Database.getUser userId
    PaymentProvider.chargeCard user.CreditCard amount
