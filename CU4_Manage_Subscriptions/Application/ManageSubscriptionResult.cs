using CestaJusta.CU4.ManageSubscriptions.Domain;

namespace CestaJusta.CU4.ManageSubscriptions.Application;

public sealed record ManageSubscriptionResult(
    bool Success,
    string Message,
    int? ProfileId,
    SubscriptionPlan? PreviousPlan,
    SubscriptionPlan? NewPlan,
    IReadOnlyList<string> GrantedPrivileges,
    SubscriptionReceipt? Receipt)
{
    public static ManageSubscriptionResult Failed(string message) =>
        new(false, message, null, null, null, Array.Empty<string>(), null);

    public static ManageSubscriptionResult Succeeded(
        int profileId,
        SubscriptionPlan previousPlan,
        SubscriptionPlan newPlan,
        IReadOnlyList<string> grantedPrivileges,
        SubscriptionReceipt receipt) =>
        new(true, "Suscripcion actualizada correctamente.", profileId, previousPlan, newPlan, grantedPrivileges, receipt);
}