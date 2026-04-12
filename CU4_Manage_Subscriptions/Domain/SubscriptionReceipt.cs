namespace CestaJusta.CU4.ManageSubscriptions.Domain;

public sealed record SubscriptionReceipt(
    string ReceiptNumber,
    int ProfileId,
    SubscriptionPlan PreviousPlan,
    SubscriptionPlan NewPlan,
    decimal Amount,
    string Currency,
    string? PaymentReference,
    DateTime IssuedAtUtc);