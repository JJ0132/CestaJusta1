-- Creo la base de datos
CREATE DATABASE MercadonaDB;
GO

-- Creo la tabla de la base de datos
USE MercadonaDB;
GO

CREATE TABLE Precio_Historico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    Fecha_Captura DATETIME NOT NULL
);