module IocDiReaderMonad.Step8

open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure

open FSharpPlus
open FSharpPlus.Data


module private Database =
    let getUser (UserId id) (connection: #ISqlConnection) : User =
        connection.QueryUser($"SELECT * FROM User AS u  WHERE u.Id = {id}")

module private PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: #IPaymentClient) : PaymentId = client.Charge card amount

let chargeUser userId amount =
    monad {
        let! (sqlConnection: #ISqlConnection) = Reader.ask
        let! (paymentClient: #IPaymentClient) = Reader.ask
        let user = Database.getUser userId sqlConnection
        let paymentId = PaymentProvider.chargeCard user.CreditCard amount paymentClient
        return paymentId

        // Can easily add something else, e.g.,
        // let! (emailClient: #IEmailClient) = Reader.ask
        // let email = EmailBody $"Your payment id is {paymentId}"
        // return Email.sendMail user.Email email emailClient
    }
