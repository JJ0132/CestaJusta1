using CestaJusta.CU1.CreateProfile.Domain;

namespace CestaJusta.CU1.CreateProfile.Application.Abstractions;

public interface IProfileRepository
{
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default);

    Task<bool> ExistsByGmailAsync(string gmail, CancellationToken cancellationToken = default);

    Task<int> InsertAsync(PerfilUsuario profile, CancellationToken cancellationToken = default);
}