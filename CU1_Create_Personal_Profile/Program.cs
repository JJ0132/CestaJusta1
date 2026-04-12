using CestaJusta.CU1.CreateProfile.Application;
using CestaJusta.CU1.CreateProfile.Infrastructure;

namespace CestaJusta.CU1.CreateProfile;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== CASO DE USO 1: CREAR PERFIL DE USUARIO ===\n");

        string connectionString = Environment.GetEnvironmentVariable("CESTAJUSTA_CONNECTION_STRING")
            ?? "Server=localhost\\SQLEXPRESS;Database=MercadonaDB;Trusted_Connection=True;TrustServerCertificate=True;";

        var repository = new SqlServerProfileRepository(connectionString);
        var passwordHasher = new Pbkdf2PasswordHasher();
        var useCase = new CreateProfileUseCase(repository, passwordHasher);

        while (true)
        {
            CreateProfileRequest request = ReadRequestFromConsole();
            CreateProfileResult result = await useCase.ExecuteAsync(request);

            Console.WriteLine();
            if (result.Success)
            {
                Console.WriteLine($"Perfil creado correctamente. Id asignado: {result.ProfileId}");
                Console.WriteLine($"Usuario: {result.Profile?.NombreUsuario}");
                Console.WriteLine($"Gmail: {result.Profile?.Gmail}");
            }
            else
            {
                Console.WriteLine($"Error: {result.Message}");
            }

            Console.WriteLine();
            Console.Write("¿Quieres crear otro perfil? (s/n): ");
            string? answer = Console.ReadLine();
            if (!string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.WriteLine();
        }
    }

    private static CreateProfileRequest ReadRequestFromConsole()
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine() ?? string.Empty;

        Console.Write("Apellidos: ");
        string apellidos = Console.ReadLine() ?? string.Empty;

        Console.Write("Nombre de usuario: ");
        string nombreUsuario = Console.ReadLine() ?? string.Empty;

        Console.Write("Gmail: ");
        string gmail = Console.ReadLine() ?? string.Empty;

        Console.Write("Contraseña: ");
        string contrasena = Console.ReadLine() ?? string.Empty;

        return new CreateProfileRequest(nombre, apellidos, nombreUsuario, gmail, contrasena);
    }
}