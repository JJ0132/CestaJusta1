namespace CestaJusta.CU4.ManageSubscriptions.Domain;

public sealed record SubscriptionProfile(
    int Id,
    string NombreUsuario,
    SubscriptionPlan CurrentPlan,
    IReadOnlyList<string> GrantedPrivileges);