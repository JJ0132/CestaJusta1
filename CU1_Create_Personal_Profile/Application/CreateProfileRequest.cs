namespace CestaJusta.CU1.CreateProfile.Application;

public sealed record CreateProfileRequest(
    string Nombre,
    string Apellidos,
    string NombreUsuario,
    string? Telefono,
    string Gmail,
    string Contrasena);