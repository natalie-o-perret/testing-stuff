[<RequireQualifiedAccess>]
module IocDiReaderMonad.Step7

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure


type Reader<'env, 'a> = Reader of ('env -> 'a)

module ReaderModule =
    let run (Reader x) = x
    let map f reader = Reader((run reader) >> f)

    let bind f reader =
        Reader(fun env ->
            let a = run reader env
            let newReader = f a
            run newReader env)

    let ask = Reader id

type private ReaderBuilder() =
    member _.Return(x) = Reader(fun _ -> x)
    member _.Bind(x, f) = ReaderModule.bind f x
    member _.Zero() = Reader(fun _ -> ())
    member _.ReturnFrom x = x

let private reader = ReaderBuilder()


module private Database =
    let getUser (UserId id) (connection: #ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: #IPaymentClient) : PaymentId = client.Charge card amount


// 👆 Usually when implementing the reader monad we create a new type to signify it, called Reader,
// in order to distinguish it from a regular function type.
// had to use Reader.ask to actually get dependencies out of the environment in this case.
// - The reason for this is because functions like Database.getUser do not return a Reader in their current form.
// - We could create a Reader on the fly by doing Reader (Database.getUser userId)
// but sometimes that can also be cumbersome, especially if we're working with client classes
// rather than functions, which is often the case.
// - So having ask in our toolkit can be a nice way to just get hold of the dependency
// and use it explicitly in the current scope.
let chargeUser userId amount =
    reader {
        let! (sqlConnection: #ISqlConnection) = ReaderModule.ask
        let! (paymentClient: #IPaymentClient) = ReaderModule.ask
        let user = Database.getUser userId sqlConnection
        let paymentId = PaymentProvider.chargeCard user.CreditCard amount paymentClient
        return paymentId

        // Can easily add something else, e.g.,
        // let! (emailClient: #IEmailClient) = Reader.ask
        // let email = EmailBody $"Your payment id is {paymentId}"
        // return Email.sendMail user.Email email emailClient
    }
