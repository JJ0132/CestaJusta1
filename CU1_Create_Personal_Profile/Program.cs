var builder = WebApplication.CreateBuilder(args);

// Desarrollo: permitimos llamadas desde el frontend estático.
// Si quieres cerrarlo, cambia a .WithOrigins("http://localhost:5500", ...) en vez de AllowAnyOrigin.
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddSingleton<IProfileRepository>(sp =>
{
    string connectionString = GetConnectionString(builder);
    return new SqlServerProfileRepository(connectionString);
});

builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<CreateProfileUseCase>();

var app = builder.Build();

app.UseCors();

app.MapPost("/api/profiles", async (CreateProfileRequest request, CreateProfileUseCase useCase, CancellationToken ct) =>
{
    CreateProfileResult result = await useCase.ExecuteAsync(request, ct);
    if (!result.Success)
    {
        return Results.BadRequest(new { ok = false, message = result.Message });
    }

    return Results.Ok(new { ok = true, profileId = result.ProfileId, nombreUsuario = result.Profile?.NombreUsuario });
});

app.MapGet("/", () => Results.Ok(new { ok = true, service = "CU1_CreateProfile", endpoints = new[] { "/api/profiles" } }));

app.Run();

static string GetConnectionString(WebApplicationBuilder builder)
{
    // Orden de prioridad:
    // 1) appsettings / user-secrets (ConnectionStrings:CestaJusta)
    // 2) variable de entorno CESTAJUSTA_CONNECTION_STRING
    // 3) fallback local (SQLEXPRESS)
    return builder.Configuration["ConnectionStrings:CestaJusta"]
        ?? Environment.GetEnvironmentVariable("CESTAJUSTA_CONNECTION_STRING")
        ?? "Server=localhost\\SQLEXPRESS;Database=MercadonaDB;Trusted_Connection=True;TrustServerCertificate=True;";
}
