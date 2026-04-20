USE MercadonaDB;
GO

IF OBJECT_ID('Perfil_Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE Perfil_Usuario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(150) NOT NULL,
        NombreUsuario NVARCHAR(80) NOT NULL,
    Telefono NVARCHAR(30) NULL,
        Gmail NVARCHAR(150) NOT NULL,
        PasswordHash NVARCHAR(256) NOT NULL,
        PasswordSalt NVARCHAR(128) NOT NULL,
        FechaCreacionUtc DATETIME2 NOT NULL CONSTRAINT DF_Perfil_Usuario_FechaCreacionUtc DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Perfil_Usuario_NombreUsuario UNIQUE (NombreUsuario),
        CONSTRAINT UQ_Perfil_Usuario_Gmail UNIQUE (Gmail)
    );
END;
GO