CREATE OR ALTER PROCEDURE PlaniFy.SP_RegistrarEmpleadoCompleto
    -- 1. Datos de Ubicación (Misma estructura que Empleador)
    @Provincia           NVARCHAR(24),
    @Canton              NVARCHAR(60),
    @Distrito            NVARCHAR(60),
    @DireccionParticular NVARCHAR(300),

    -- 2. Datos Personales (Misma estructura que Empleador)
    @Nombre              NVARCHAR(40),
    @SegundoNombre       NVARCHAR(40) = NULL,
    @Apellidos           NVARCHAR(40),
    @Correo              NVARCHAR(100),
    @Cedula              CHAR(9),
    @Telefono            INT,
    @FechaNacimiento     DATE,

    -- 3. Datos Laborales (Exclusivos de Empleado)
    @Departamento        NVARCHAR(40),
    @TipoContrato        NVARCHAR(50),
    @Puesto              NVARCHAR(40),
    @Salario             INT,
    @Iban                NVARCHAR(60),
    @IdEmpresa           INT,

    -- 4. Seguridad (Solo Token, Password NULA)
    @TokenHash           NVARCHAR(128),
    @TokenExpires        DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

            -- Validación de concurrencia
            IF EXISTS (SELECT 1 FROM PlaniFy.Persona WHERE Correo = @Correo OR Cedula = @Cedula)
            BEGIN
                THROW 50001, 'El correo o la cédula ya están registrados.', 1;
            END

            -- A. Insertar Dirección
            INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
            VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular);
            
            DECLARE @NewDireccionId INT = SCOPE_IDENTITY();

            -- B. Insertar Persona
            INSERT INTO PlaniFy.Persona (
                Nombre, SegundoNombre, Apellidos, Correo, Cedula, 
                Telefono, FechaNacimiento, Rol, idDireccion
            )
            VALUES (
                @Nombre, @SegundoNombre, @Apellidos, @Correo, @Cedula, 
                @Telefono, @FechaNacimiento, 'Empleado', @NewDireccionId
            );

            DECLARE @NewPersonaId INT = SCOPE_IDENTITY();

            -- C. Insertar Ficha de Empleado
            -- Nota: Dejamos Contrasena NULL aquí también si la columna existe en tabla Empleado
            INSERT INTO PlaniFy.Empleado (
                idPersona, Departamento, TipoContrato, TipoSalario, 
                Puesto, FechaContratacion, Salario, iban, 
                Contrasena, idEmpresa, Estado
            )
            VALUES (
                @NewPersonaId, @Departamento, @TipoContrato, 'Mensual', 
                @Puesto, GETDATE(), @Salario, @Iban, 
                NULL, @IdEmpresa, 'Activo'
            );

            -- D. Insertar Usuario (SIN Contraseña)
            -- Aquí está la diferencia clave: Contrasena = NULL
            INSERT INTO PlaniFy.Usuario (
                idPersona, TipoUsuario, Contrasena, 
                VerificationTokenHash, VerificationTokenExpires, IsVerified
            )
            VALUES (
                @NewPersonaId, 'Empleado', NULL, 
                @TokenHash, @TokenExpires, 0
            );

        COMMIT TRANSACTION;

        SELECT @NewPersonaId AS Id;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO