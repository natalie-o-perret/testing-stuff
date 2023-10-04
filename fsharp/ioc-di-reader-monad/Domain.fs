namespace IocDiReaderMonad.Domain


type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type EmailAddress = EmailAddress of string
type UserId = UserId of string
type PaymentId = PaymentId of string

type User =
    { Id: UserId
      CreditCard: CreditCard
      EmailAddress: EmailAddress }
