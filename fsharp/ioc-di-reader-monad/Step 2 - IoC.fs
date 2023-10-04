[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step2

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


module private Database =

    let getUser (connection: ISqlConnection) (UserId id) : User =
        connection.QueryUser($"SELECT * FROM User AS u WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (client: IPaymentClient) (card: CreditCard) amount = client.Charge card amount


// Better than Step 1 because: We’ve just supplied the necessary clients as parameters and passed them along to the
// function calls that need them.
// BUT...
// - Number of dependencies grows the number of function parameters can become unruly.
// - Most applications have some degree of layering to them.
// - More layers, to break down and isolate the responsibilities of individual functions, we start needing to
// pass some dependencies down through many layers.
// => typical of any IoC solution, once you flip those dependencies it cascades right through all the layers of
// your application
// => It’s turtles inverted dependencies all the way down 🤐
let chargeUser sqlConnection paymentClient userId amount =
    let user = Database.getUser sqlConnection userId
    PaymentProvider.chargeCard paymentClient user.CreditCard amount
