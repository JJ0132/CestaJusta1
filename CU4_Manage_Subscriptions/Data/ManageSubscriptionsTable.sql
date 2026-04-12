USE MercadonaDB;
GO

IF COL_LENGTH('Perfil_Usuario', 'PlanSuscripcion') IS NULL
BEGIN
    ALTER TABLE Perfil_Usuario
    ADD PlanSuscripcion NVARCHAR(20) NOT NULL CONSTRAINT DF_Perfil_Usuario_PlanSuscripcion DEFAULT 'Basic';
END;
GO

IF COL_LENGTH('Perfil_Usuario', 'PrivilegiosSuscripcion') IS NULL
BEGIN
    ALTER TABLE Perfil_Usuario
    ADD PrivilegiosSuscripcion NVARCHAR(500) NOT NULL CONSTRAINT DF_Perfil_Usuario_PrivilegiosSuscripcion DEFAULT 'menus_semanales,ajuste_presupuesto,filtros_basicos,necesidades_medicas';
END;
GO

IF COL_LENGTH('Perfil_Usuario', 'UltimaActualizacionSuscripcionUtc') IS NULL
BEGIN
    ALTER TABLE Perfil_Usuario
    ADD UltimaActualizacionSuscripcionUtc DATETIME2 NULL;
END;
GO

UPDATE Perfil_Usuario
SET PlanSuscripcion = 'Basic',
    PrivilegiosSuscripcion = 'menus_semanales,ajuste_presupuesto,filtros_basicos,necesidades_medicas'
WHERE PlanSuscripcion IS NULL;
GO