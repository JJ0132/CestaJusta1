# CU1 – Crear perfil (Web)

Este caso de uso ahora se ejecuta como **mini webapp**:

- Sirve un frontend básico en `register.html`.
- Expone un endpoint `POST /api/profiles`.
- Guarda el perfil en **SQL Server** en la tabla `Perfil_Usuario`.

## Estructura

- `Program.cs`: backend web (Minimal API).
- `wwwroot/`: frontend básico (HTML/CSS/JS).
- `Application/`: caso de uso, request/result y contratos.
- `Domain/`: entidad de perfil.
- `Infrastructure/`: persistencia SQL Server + hash de contraseña.
- `Data/CreateProfilesTable.sql`: script de creación de tabla.

## Qué guarda

- Nombre
- Apellidos
- Nombre de usuario
- Teléfono (opcional)
- Gmail
- PasswordHash + PasswordSalt
- Fecha de creación (UTC)

## Requisitos

- .NET 9 SDK (el proyecto compila con `net9.0`).
- SQL Server local en `localhost\\SQLEXPRESS` (o el que indiques en la cadena de conexión).
- Base de datos `MercadonaDB`.

## Base de datos

Ejecuta `Data/CreateProfilesTable.sql` en tu SQL Server.

Si ya tenías la tabla creada, asegúrate de tener la columna:

```sql
Telefono NVARCHAR(30) NULL
```

## Configuración

El backend toma la cadena de conexión desde:

- `ConnectionStrings:CestaJusta` (configuración), o
- variable de entorno `CESTAJUSTA_CONNECTION_STRING`, o
- fallback a `Server=localhost\\SQLEXPRESS;Database=MercadonaDB;Trusted_Connection=True;TrustServerCertificate=True;`.

## Ejecutar

Desde esta carpeta:

```powershell
dotnet run
```

Luego abre:

- `http://localhost:5000/register.html`

## Notas

- La validación de correo exige `@gmail.com` (ver `Application/CreateProfileUseCase.cs`).
- El frontend principal (en `FrontEnd/figma/index.html`) enlaza a `http://localhost:5000/register.html`.