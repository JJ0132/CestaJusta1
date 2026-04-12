namespace CestaJusta.CU1.CreateProfile.Domain;

public sealed record PerfilUsuario(
    int? Id,
    string Nombre,
    string Apellidos,
    string NombreUsuario,
    string Gmail,
    string PasswordHash,
    string PasswordSalt,
    DateTime FechaCreacionUtc);