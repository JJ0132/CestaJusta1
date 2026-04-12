namespace CestaJusta.CU4.ManageSubscriptions.Application.Abstractions;

public interface IPaymentGateway
{
    Task<PaymentAuthorizationResult> AuthorizeAsync(
        PaymentDetails paymentDetails,
        decimal amount,
        CancellationToken cancellationToken = default);
}

public sealed record PaymentAuthorizationResult(
    bool Approved,
    string? AuthorizationCode,
    string? ErrorMessage)
{
    public static PaymentAuthorizationResult ApprovedResult(string authorizationCode) =>
        new(true, authorizationCode, null);

    public static PaymentAuthorizationResult Rejected(string errorMessage) =>
        new(false, null, errorMessage);
}