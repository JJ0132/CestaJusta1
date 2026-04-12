# CU1 - Create Personal Profile

Este caso de uso crea un perfil de usuario y lo guarda en SQL Server siguiendo el flujo:

UI -> Backend -> Database

## Estructura

- `Program.cs`: interfaz de consola para capturar los datos.
- `Application/`: caso de uso, petición, resultado y contratos.
- `Domain/`: entidad de perfil.
- `Infrastructure/`: persistencia en SQL Server y hash de contraseña.
- `Data/CreateProfilesTable.sql`: script de creación de tabla.

## Requisitos

- .NET 10
- SQL Server local en `localhost\\SQLEXPRESS`
- Base de datos `MercadonaDB`

## Preparación de la tabla

Ejecuta `Data/CreateProfilesTable.sql` sobre `MercadonaDB`.

## Ejecutar

Desde esta carpeta:

```bash
dotnet run
```

Si necesitas otra cadena de conexión, define la variable de entorno `CESTAJUSTA_CONNECTION_STRING`.