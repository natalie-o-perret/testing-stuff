[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step6

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


let private inject f valueThatNeedsDep =
    fun (deps: 'deps) ->
        let value = valueThatNeedsDep deps
        f value deps

type private InjectorBuilder() =
    member _.Return(x) = fun _ -> x
    member _.Bind(x, f) = inject f x
    member _.Zero() = fun _ -> ()
    member _.ReturnFrom x = x

let private injector = InjectorBuilder()

module private Database =
    let getUser (UserId id) (connection: #ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: #IPaymentClient) : PaymentId = client.Charge card amount


// Better than Step 5: going full circle and somehow back to naive step 1 without the hardcoded limitation except
// - Now, all of the control is properly inverted.
// - And, the transitive dependencies are nicely hidden from sight they’re still being type checked.
let chargeUser userId amount =
    injector {
        let! user = Database.getUser userId
        let! paymentId = PaymentProvider.chargeCard user.CreditCard amount
        return paymentId

        // Can easily add something else, e.g.,
        // let email = EmailBody $"Your payment id is {paymentId}"
        // return! Email.sendMail user.Email email
    }
