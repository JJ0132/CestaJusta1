using CestaJusta.CU4.ManageSubscriptions.Domain;

namespace CestaJusta.CU4.ManageSubscriptions.Application.Abstractions;

public interface IProfileSubscriptionRepository
{
    Task<SubscriptionProfile?> GetByIdAsync(int profileId, CancellationToken cancellationToken = default);

    Task UpdateSubscriptionAsync(
        int profileId,
        SubscriptionPlan plan,
        IReadOnlyList<string> privileges,
        DateTime updatedAtUtc,
        CancellationToken cancellationToken = default);
}