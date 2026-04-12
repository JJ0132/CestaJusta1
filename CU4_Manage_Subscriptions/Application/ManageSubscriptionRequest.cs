using CestaJusta.CU4.ManageSubscriptions.Domain;

namespace CestaJusta.CU4.ManageSubscriptions.Application;

public sealed record ManageSubscriptionRequest(
    int ProfileId,
    SubscriptionPlan TargetPlan,
    PaymentDetails PaymentDetails);

public sealed record PaymentDetails(
    string CardholderName,
    string PaymentReference,
    bool ApprovePayment);