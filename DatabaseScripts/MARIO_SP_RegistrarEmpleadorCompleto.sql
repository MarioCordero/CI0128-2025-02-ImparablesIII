CREATE OR ALTER PROCEDURE PlaniFy.SP_RegistrarEmpleadorCompleto
    -- 1. Datos de Ubicación
    @Provincia           NVARCHAR(24),
    @Canton              NVARCHAR(60),
    @Distrito            NVARCHAR(60),
    @DireccionParticular NVARCHAR(300),

    -- 2. Datos Personales
    @Nombre              NVARCHAR(40),
    @SegundoNombre       NVARCHAR(40) = NULL,
    @Apellidos           NVARCHAR(40),
    @Correo              NVARCHAR(100),
    @Cedula              CHAR(9),
    @Telefono            INT,
    @FechaNacimiento     DATE,

    -- 3. Seguridad (Autenticación Inmediata)
    @ContrasenaHash      NVARCHAR(100),
    @TokenHash           NVARCHAR(128),
    @TokenExpires        DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- Rollback automático si hay error severo

    BEGIN TRY
        BEGIN TRANSACTION;

            -- Validación de concurrencia (Por si dos personas se registran al mismo milisegundo)
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
                @Telefono, @FechaNacimiento, 'Empleador', @NewDireccionId
            );

            DECLARE @NewPersonaId INT = SCOPE_IDENTITY();

            -- C. Insertar Usuario (CON Contraseña)
            INSERT INTO PlaniFy.Usuario (
                idPersona, TipoUsuario, Contrasena, 
                VerificationTokenHash, VerificationTokenExpires, IsVerified
            )
            VALUES (
                @NewPersonaId, 'Empleador', @ContrasenaHash, 
                @TokenHash, @TokenExpires, 0
            );

        COMMIT TRANSACTION;

        -- Retornamos el ID para loguear en backend
        SELECT @NewPersonaId AS Id;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        -- Propagar el error exacto a C#
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO
SELECT COUNT(*) FROM PlaniFy.Direccion;
SELECT * FROM PlaniFy.Direccion;
----------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------- DEMOSTRAR QUE FUNCIONA --------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE PlaniFy.SP_RegistrarEmpleadorCompleto
    @Provincia           NVARCHAR(24),
    @Canton              NVARCHAR(60),
    @Distrito            NVARCHAR(60),
    @DireccionParticular NVARCHAR(300),
    @Nombre              NVARCHAR(40),
    @SegundoNombre       NVARCHAR(40) = NULL,
    @Apellidos           NVARCHAR(40),
    @Correo              NVARCHAR(100),
    @Cedula              CHAR(9),
    @Telefono            INT,
    @FechaNacimiento     DATE,
    @ContrasenaHash      NVARCHAR(100),
    @TokenHash           NVARCHAR(128),
    @TokenExpires        DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; 

    BEGIN TRY
        BEGIN TRANSACTION;

            -- 1. Insertar Dirección (SE EJECUTA)
            INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
            VALUES (@Provincia, @Canton, @Distrito, @DireccionParticular);
            DECLARE @NewDireccionId INT = SCOPE_IDENTITY();

            -- 2. Insertar Persona (SE EJECUTA)
            INSERT INTO PlaniFy.Persona (Nombre, SegundoNombre, Apellidos, Correo, Cedula, Telefono, FechaNacimiento, Rol, idDireccion)
            VALUES (@Nombre, @SegundoNombre, @Apellidos, @Correo, @Cedula, @Telefono, @FechaNacimiento, 'Empleador', @NewDireccionId);
            DECLARE @NewPersonaId INT = SCOPE_IDENTITY();

            -- FORZAR ERROR
            DECLARE @ErrorProvocado INT = 1 / 0;

            -- 3. Insertar Usuario (NUNCA LLEGA AQUÍ)
            INSERT INTO PlaniFy.Usuario (idPersona, TipoUsuario, Contrasena, VerificationTokenHash, VerificationTokenExpires, IsVerified)
            VALUES (@NewPersonaId, 'Empleador', @ContrasenaHash, @TokenHash, @TokenExpires, 0);

        COMMIT TRANSACTION;
        SELECT @NewPersonaId AS Id;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = 'ERROR DEMOSTRATIVO: ' + ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO
SELECT COUNT(*) FROM PlaniFy.Direccion;

-- SWAGGER TEST DATA
-- {
--   "nombre": "Mario",
--   "segundoNombre": "Gabriel",
--   "primerApellido": "Cordero",
--   "segundoApellido": "Méndez",
--   "cedula": "115430987",
--   "email": "mariogabriel.cordero@ucr.ac.cr",
--   "telefono": 88885555,
--   "fechaNacimiento": "1998-05-15T00:00:00.000Z",
--   "password": "Hola123!!",
--   "confirmPassword": "Hola123!!",
--   "provincia": "San José",
--   "canton": "Montes de Oca",
--   "distrito": "San Pedro",
--   "direccionParticular": "De la Facultad de Derecho, 100m Este y 25m Sur, casa color beige"
-- }