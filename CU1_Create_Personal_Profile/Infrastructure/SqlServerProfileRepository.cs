using CestaJusta.CU1.CreateProfile.Application.Abstractions;
using CestaJusta.CU1.CreateProfile.Domain;
using Microsoft.Data.SqlClient;

namespace CestaJusta.CU1.CreateProfile.Infrastructure;

public sealed class SqlServerProfileRepository : IProfileRepository
{
    private readonly SqlServerConnectionFactory connectionFactory;

    public SqlServerProfileRepository(string connectionString)
        : this(new SqlServerConnectionFactory(connectionString))
    {
    }

    public SqlServerProfileRepository(SqlServerConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default)
    {
        (bool nombreUsuarioExists, _) = await CheckDuplicatesAsync(nombreUsuario, gmail: string.Empty, cancellationToken);
        return nombreUsuarioExists;
    }

    public async Task<bool> ExistsByGmailAsync(string gmail, CancellationToken cancellationToken = default)
    {
        (_, bool gmailExists) = await CheckDuplicatesAsync(nombreUsuario: string.Empty, gmail, cancellationToken);
        return gmailExists;
    }

    public async Task<(bool NombreUsuarioExists, bool GmailExists)> CheckDuplicatesAsync(
        string nombreUsuario,
        string gmail,
        CancellationToken cancellationToken = default)
    {
        // Nota: permitimos pasar uno de los dos par3metros vac6o. En ese caso, solo se eval3a el otro.
        const string sql = """
            SELECT
                CASE WHEN @NombreUsuario <> '' AND EXISTS (
                    SELECT 1 FROM Perfil_Usuario WHERE NombreUsuario = @NombreUsuario
                ) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS NombreUsuarioExists,
                CASE WHEN @Gmail <> '' AND EXISTS (
                    SELECT 1 FROM Perfil_Usuario WHERE Gmail = @Gmail
                ) THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS GmailExists;
            """;

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario ?? string.Empty);
        command.Parameters.AddWithValue("@Gmail", gmail ?? string.Empty);

        await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return (false, false);
        }

        bool nombreUsuarioExists = reader.GetBoolean(0);
        bool gmailExists = reader.GetBoolean(1);
        return (nombreUsuarioExists, gmailExists);
    }

    public async Task<int> InsertAsync(PerfilUsuario profile, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Perfil_Usuario
                (Nombre, Apellidos, NombreUsuario, Telefono, Gmail, PasswordHash, PasswordSalt, FechaCreacionUtc)
            OUTPUT INSERTED.Id
            VALUES
                (@Nombre, @Apellidos, @NombreUsuario, @Telefono, @Gmail, @PasswordHash, @PasswordSalt, @FechaCreacionUtc);
            """;

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Nombre", profile.Nombre);
        command.Parameters.AddWithValue("@Apellidos", profile.Apellidos);
        command.Parameters.AddWithValue("@NombreUsuario", profile.NombreUsuario);
    command.Parameters.AddWithValue("@Telefono", (object?)profile.Telefono ?? DBNull.Value);
        command.Parameters.AddWithValue("@Gmail", profile.Gmail);
        command.Parameters.AddWithValue("@PasswordHash", profile.PasswordHash);
        command.Parameters.AddWithValue("@PasswordSalt", profile.PasswordSalt);
        command.Parameters.AddWithValue("@FechaCreacionUtc", profile.FechaCreacionUtc);

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}