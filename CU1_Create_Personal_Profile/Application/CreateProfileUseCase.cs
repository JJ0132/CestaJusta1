using System.Text.RegularExpressions;
using CestaJusta.CU1.CreateProfile.Application.Abstractions;
using CestaJusta.CU1.CreateProfile.Domain;

namespace CestaJusta.CU1.CreateProfile.Application;

public sealed class CreateProfileUseCase
{
    private static readonly Regex GmailRegex = new(@"^[^@\s]+@gmail\.com$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly IProfileRepository profileRepository;
    private readonly IPasswordHasher passwordHasher;

    public CreateProfileUseCase(IProfileRepository profileRepository, IPasswordHasher passwordHasher)
    {
        this.profileRepository = profileRepository;
        this.passwordHasher = passwordHasher;
    }

    public async Task<CreateProfileResult> ExecuteAsync(CreateProfileRequest request, CancellationToken cancellationToken = default)
    {
        string? validationError = Validate(request);
        if (validationError is not null)
        {
            return CreateProfileResult.Failed(validationError);
        }

        string nombre = request.Nombre.Trim();
        string apellidos = request.Apellidos.Trim();
        string nombreUsuario = request.NombreUsuario.Trim();
    string? telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : request.Telefono.Trim();
        string gmail = request.Gmail.Trim().ToLowerInvariant();

        (bool nombreUsuarioExists, bool gmailExists) = await profileRepository.CheckDuplicatesAsync(nombreUsuario, gmail, cancellationToken);
        if (nombreUsuarioExists)
        {
            return CreateProfileResult.Failed("El nombre de usuario ya existe.");
        }

        if (gmailExists)
        {
            return CreateProfileResult.Failed("El Gmail ya est registrado.");
        }

        PasswordHashResult password = passwordHasher.Hash(request.Contrasena);
        PerfilUsuario profile = new(
            Id: null,
            Nombre: nombre,
            Apellidos: apellidos,
            NombreUsuario: nombreUsuario,
            Telefono: telefono,
            Gmail: gmail,
            PasswordHash: password.Hash,
            PasswordSalt: password.Salt,
            FechaCreacionUtc: DateTime.UtcNow);

        int profileId = await profileRepository.InsertAsync(profile, cancellationToken);
        PerfilUsuario createdProfile = profile with { Id = profileId };

        return CreateProfileResult.Succeeded(profileId, createdProfile);
    }

    private static string? Validate(CreateProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre) ||
            string.IsNullOrWhiteSpace(request.Apellidos) ||
            string.IsNullOrWhiteSpace(request.NombreUsuario) ||
            string.IsNullOrWhiteSpace(request.Gmail) ||
            string.IsNullOrWhiteSpace(request.Contrasena))
        {
            return "Todos los campos son obligatorios.";
        }

        if (!GmailRegex.IsMatch(request.Gmail.Trim()))
        {
            return "El correo debe ser un Gmail válido (ej: usuario@gmail.com).";
        }

        if (request.Contrasena.Trim().Length < 6)
        {
            return "La contraseña debe tener al menos 6 caracteres.";
        }

        return null;
    }
}