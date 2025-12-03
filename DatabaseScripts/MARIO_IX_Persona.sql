-- DEMOSTRACIÓN DE RENDIMIENTO: ÍNDICE POR CORREO (PlaniFy)
SELECT * FROM PlaniFy.Persona;
SET NOCOUNT ON; -- Evita mensajes adicionales que interfieren con la salida

-- ============================================================================
-- CREAR ÍNDICE
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Persona_Correo' AND object_id = OBJECT_ID('PlaniFy.Persona'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Persona_Correo
    ON PlaniFy.Persona (Correo)
    INCLUDE (Id, Nombre, Apellidos, Rol);
END
ELSE
BEGIN
    PRINT 'Error: El índice ya existe.';
END
GO

-- ============================================================================
-- GENERAR BASURA EN LA BASE DE DATOS PARA QUE LE CUESTE BUSCAR
-- ============================================================================
DECLARE @TotalRows INT = (SELECT COUNT(*) FROM PlaniFy.Persona WHERE Correo LIKE '%@demo.test%');
IF @TotalRows < 10000
BEGIN
    PRINT '    Generando 10,000 usuarios prueba... No sea intenso, espere...';
    DECLARE @DirId INT; -- Dirección temporal para los dummies
    IF NOT EXISTS (SELECT 1 FROM PlaniFy.Direccion WHERE DireccionParticular = 'DIR_DEMO_PERFORMANCE')
    BEGIN
        INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
        VALUES ('San José', 'Demo', 'Demo', 'DIR_DEMO_PERFORMANCE');
    END
    SELECT @DirId = Id FROM PlaniFy.Direccion WHERE DireccionParticular = 'DIR_DEMO_PERFORMANCE';

    DECLARE @i INT = 0;
    WHILE @i < 10000 -- While de inserción
    BEGIN
        BEGIN TRY
            INSERT INTO PlaniFy.Persona (
                Nombre, Apellidos, Correo, Cedula, 
                Telefono, FechaNacimiento, Rol, idDireccion
            )
            VALUES (
                'UserDemo', 
                'Performance', 
                'user_' + CAST(@i AS VARCHAR) + '@demo.test', -- Correo único
                CAST(900000000 + @i AS VARCHAR), -- Cédula dummy alta para no chocar
                88888888, 
                GETDATE(), 
                'Empleado', 
                @DirId
            );
        END TRY
        BEGIN CATCH
            -- Ignorar errores de duplicados (Fijo hay)
        END CATCH
        SET @i = @i + 1;
    END
    PRINT 'Datos basura generados correctamente.';
END
ELSE
BEGIN
    PRINT 'Ya hay datos de prueba suficientes.';
END
GO

-- ============================================================================
-- COMPARACION
-- ============================================================================
PRINT ' ';
PRINT '==================================================================';
PRINT 'EJECUTANDO COMPARATIVA';
PRINT '==================================================================';

SET STATISTICS IO ON; -- estadísticas para ver las "Logical Reads"
SET STATISTICS TIME ON;

-- SIN ÍNDICE (Table Scan forzando índice 0)
PRINT ' ';
PRINT '----------------------- BÚSQUEDA SIN ÍNDICE (Table Scan - Lento) -----------------------';
SELECT Id, Nombre, Rol, Correo
FROM PlaniFy.Persona WITH (INDEX(0)) -- Ignorar indice
WHERE Correo = 'user_5000@demo.test';

-- CON ÍNDICE (Index Seek)
PRINT ' ';
PRINT '----------------------- BÚSQUEDA CON ÍNDICE (Index Seek - Rápido) -----------------------';
SELECT Id, Nombre, Rol, Correo
FROM PlaniFy.Persona -- Usa el IX automaticamente
WHERE Correo = 'user_5000@demo.test';

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
GO


PRINT '>>> Limpiando datos basura...';
DELETE FROM PlaniFy.Persona WHERE Correo LIKE '%@demo.test';
DELETE FROM PlaniFy.Direccion WHERE DireccionParticular = 'DIR_DEMO_PERFORMANCE';
PRINT 'Limpieza completada.';