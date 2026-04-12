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
        const string sql = "SELECT COUNT(1) FROM Perfil_Usuario WHERE NombreUsuario = @NombreUsuario;";

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

        int count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return count > 0;
    }

    public async Task<bool> ExistsByGmailAsync(string gmail, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(1) FROM Perfil_Usuario WHERE Gmail = @Gmail;";

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Gmail", gmail);

        int count = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        return count > 0;
    }

    public async Task<int> InsertAsync(PerfilUsuario profile, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Perfil_Usuario
                (Nombre, Apellidos, NombreUsuario, Gmail, PasswordHash, PasswordSalt, FechaCreacionUtc)
            OUTPUT INSERTED.Id
            VALUES
                (@Nombre, @Apellidos, @NombreUsuario, @Gmail, @PasswordHash, @PasswordSalt, @FechaCreacionUtc);
            """;

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@Nombre", profile.Nombre);
        command.Parameters.AddWithValue("@Apellidos", profile.Apellidos);
        command.Parameters.AddWithValue("@NombreUsuario", profile.NombreUsuario);
        command.Parameters.AddWithValue("@Gmail", profile.Gmail);
        command.Parameters.AddWithValue("@PasswordHash", profile.PasswordHash);
        command.Parameters.AddWithValue("@PasswordSalt", profile.PasswordSalt);
        command.Parameters.AddWithValue("@FechaCreacionUtc", profile.FechaCreacionUtc);

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}