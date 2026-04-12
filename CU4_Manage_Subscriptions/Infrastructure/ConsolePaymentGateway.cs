using CestaJusta.CU4.ManageSubscriptions.Application;
using CestaJusta.CU4.ManageSubscriptions.Application.Abstractions;

namespace CestaJusta.CU4.ManageSubscriptions.Infrastructure;

public sealed class ConsolePaymentGateway : IPaymentGateway
{
    public Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentDetails paymentDetails,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (!paymentDetails.ApprovePayment)
        {
            return Task.FromResult(PaymentAuthorizationResult.Rejected("Pago rechazado por el usuario en la pasarela."));
        }

        string authorizationCode = $"AUTH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Math.Abs(paymentDetails.PaymentReference.GetHashCode()):X}";
        return Task.FromResult(PaymentAuthorizationResult.ApprovedResult(authorizationCode));
    }
}