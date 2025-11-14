CREATE OR ALTER PROCEDURE PlaniFy.SP_ActualizarBeneficio
    @CompanyId INT,
    @OriginalName NVARCHAR(20),
    @NewName NVARCHAR(20),
    @Descripcion NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validar que el beneficio original existe
        IF NOT EXISTS (
            SELECT 1 
            FROM PlaniFy.Beneficio 
            WHERE idEmpresa = @CompanyId AND Nombre = @OriginalName
        )
        BEGIN
            RAISERROR('Beneficio no encontrado', 16, 1);
            RETURN;
        END

        -- Validar que el nuevo nombre no existe (si cambió)
        IF @OriginalName != @NewName 
           AND EXISTS (
               SELECT 1 
               FROM PlaniFy.Beneficio 
               WHERE idEmpresa = @CompanyId AND Nombre = @NewName
           )
        BEGIN
            RAISERROR('Ya existe un beneficio con este nombre para esta empresa', 16, 1);
            RETURN;
        END

        -- Deshabilitar la FK específica para permitir el cambio de nombre
        ALTER TABLE PlaniFy.BeneficioEmpleado 
        NOCHECK CONSTRAINT FK__BeneficioEmplead__1E05700A;

        -- Actualizar el beneficio principal
        UPDATE PlaniFy.Beneficio 
        SET Nombre = @NewName, 
            Descripcion = @Descripcion
        WHERE idEmpresa = @CompanyId AND Nombre = @OriginalName;

        -- Actualizar los registros relacionados si cambió el nombre
        IF @OriginalName != @NewName
        BEGIN
            UPDATE PlaniFy.BeneficioEmpleado 
            SET NombreBeneficio = @NewName
            WHERE idEmpresa = @CompanyId AND NombreBeneficio = @OriginalName;
        END

        -- Rehabilitar la FK
        ALTER TABLE PlaniFy.BeneficioEmpleado 
        WITH CHECK CHECK CONSTRAINT FK__BeneficioEmplead__1E05700A;
    END TRY
    BEGIN CATCH
        -- En caso de error, reactivar la constraint para no dejarla deshabilitada
        ALTER TABLE PlaniFy.BeneficioEmpleado 
        WITH CHECK CHECK CONSTRAINT FK__BeneficioEmplead__1E05700A;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO