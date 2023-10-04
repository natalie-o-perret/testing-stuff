module IocDiReaderMonad.Infrastructure

open IocDiReaderMonad.Domain


type ISqlConnection =
    abstract QueryUser: string -> User

type SqlConnection(_connectionString) =
    interface ISqlConnection with
        member _.QueryUser(_queryString) =
            { Id = UserId "User 42"
              CreditCard =
                { Number = "42"
                  Expiry = "4242/12/25"
                  Cvv = "42" }
              EmailAddress = EmailAddress "42@42.42" }


type IPaymentClient =
    abstract Charge: CreditCard -> float -> PaymentId

type PaymentClient(_apiSecret) =
    interface IPaymentClient with
        member _.Charge _creditCard _amount =
            "0123456789ABCDEF" |> string |> PaymentId
