using CestaJusta.CU4.ManageSubscriptions.Application.Abstractions;
using CestaJusta.CU4.ManageSubscriptions.Domain;
using Microsoft.Data.SqlClient;

namespace CestaJusta.CU4.ManageSubscriptions.Infrastructure;

public sealed class SqlServerSubscriptionRepository : IProfileSubscriptionRepository
{
    private readonly SqlServerConnectionFactory connectionFactory;

    public SqlServerSubscriptionRepository(string connectionString)
        : this(new SqlServerConnectionFactory(connectionString))
    {
    }

    public SqlServerSubscriptionRepository(SqlServerConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<SubscriptionProfile?> GetByIdAsync(int profileId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, NombreUsuario, PlanSuscripcion, PrivilegiosSuscripcion
            FROM Perfil_Usuario
            WHERE Id = @ProfileId;
            """;

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProfileId", profileId);

        await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        SubscriptionPlan currentPlan = ParsePlan(reader.GetString(2));
        IReadOnlyList<string> privileges = ParsePrivileges(reader.GetString(3));

        return new SubscriptionProfile(
            Id: reader.GetInt32(0),
            NombreUsuario: reader.GetString(1),
            CurrentPlan: currentPlan,
            GrantedPrivileges: privileges);
    }

    public async Task UpdateSubscriptionAsync(
        int profileId,
        SubscriptionPlan plan,
        IReadOnlyList<string> privileges,
        DateTime updatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Perfil_Usuario
            SET PlanSuscripcion = @PlanSuscripcion,
                PrivilegiosSuscripcion = @PrivilegiosSuscripcion,
                UltimaActualizacionSuscripcionUtc = @UltimaActualizacionSuscripcionUtc
            WHERE Id = @ProfileId;
            """;

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProfileId", profileId);
        command.Parameters.AddWithValue("@PlanSuscripcion", ToDatabaseValue(plan));
        command.Parameters.AddWithValue("@PrivilegiosSuscripcion", string.Join(",", privileges));
        command.Parameters.AddWithValue("@UltimaActualizacionSuscripcionUtc", updatedAtUtc);

        int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException("No se pudo actualizar la suscripcion del perfil.");
        }
    }

    private static SubscriptionPlan ParsePlan(string planValue) => planValue.ToLowerInvariant() switch
    {
        "plus" => SubscriptionPlan.Plus,
        "familiar" => SubscriptionPlan.Familiar,
        _ => SubscriptionPlan.Basic
    };

    private static string ToDatabaseValue(SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Plus => "Plus",
        SubscriptionPlan.Familiar => "Familiar",
        _ => "Basic"
    };

    private static IReadOnlyList<string> ParsePrivileges(string privilegesValue)
    {
        if (string.IsNullOrWhiteSpace(privilegesValue))
        {
            return Array.Empty<string>();
        }

        return privilegesValue.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}