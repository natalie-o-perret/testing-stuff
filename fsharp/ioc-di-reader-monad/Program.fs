open FSharpPlus.Data
open IocDiReaderMonad
open IocDiReaderMonad.Domain
open IocDiReaderMonad.Infrastructure

let userId = (UserId "Michelle 42")
let amount = 42.42
let sqlConnection: ISqlConnection = SqlConnection("my-connection-string")
let paymentClient: IPaymentClient = PaymentClient("my-payment-api-secret")

type IDeps =
    inherit IPaymentClient
    inherit ISqlConnection
    // inherit IEmailClient

let deps =
    { new IDeps with
        member _.Charge card amount = paymentClient.Charge card amount
        member _.QueryUser x = sqlConnection.QueryUser(x)
        // member _.SendMail address body = // create SMTP client and call it
    }

[ Step1.chargeUser userId amount
  Step2.chargeUser sqlConnection paymentClient <|| (userId, amount)
  Step3.chargeUser userId amount <|| (sqlConnection, paymentClient)
  Step4.chargeUser userId amount deps
  Step5.chargeUser userId amount deps
  Step6.chargeUser userId amount deps
  Step7.ReaderModule.run (Step7.chargeUser userId amount) deps
  Reader.run (Step8.chargeUser userId amount) deps ]
|> List.iter (printfn "%A")
