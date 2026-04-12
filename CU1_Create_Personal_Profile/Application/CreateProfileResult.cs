using CestaJusta.CU1.CreateProfile.Domain;

namespace CestaJusta.CU1.CreateProfile.Application;

public sealed record CreateProfileResult(
    bool Success,
    string Message,
    int? ProfileId,
    PerfilUsuario? Profile)
{
    public static CreateProfileResult Failed(string message) => new(false, message, null, null);

    public static CreateProfileResult Succeeded(int profileId, PerfilUsuario profile) =>
        new(true, "Perfil creado y guardado correctamente.", profileId, profile);
}