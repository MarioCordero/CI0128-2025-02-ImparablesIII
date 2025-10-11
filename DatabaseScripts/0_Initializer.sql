-- ===================================
-- SCRIPT PARA ELIMINAR LA BASE DE DATOS PLANIFY (IGNORAR)
-- ===================================

-- Cerrar todas las conexiones activas a la base de datos (si existe)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PlaniFy')
BEGIN
    ALTER DATABASE PlaniFy SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PlaniFy;
    PRINT 'Base de datos PlaniFy eliminada correctamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos PlaniFy no existe.';
END
GO

-- Verificar que la base de datos fue eliminada
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PlaniFy')
BEGIN
    PRINT 'Confirmado: La base de datos PlaniFy ha sido eliminada.';
END
ELSE
BEGIN
    PRINT 'Error: La base de datos PlaniFy aún existe.';
END
GO


-- ===================================
-- SCRIPT DE BASE DE DATOS PLANIFY
-- ===================================

-------------------------- PARA USAR EL AZURE DEL PROFE
-- Crear esquema si no existe
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'PlaniFy')
BEGIN
    EXEC('CREATE SCHEMA PlaniFy');
END;
GO

-------------------------- PARA USAR SQL SERVER LOCAL
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'PlaniFy')
BEGIN
    CREATE DATABASE PlaniFy;
    PRINT 'Base de datos PlaniFy creada correctamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos PlaniFy ya existe.';
END
GO

-- Cambiar a la base de datos PlaniFy
USE PlaniFy;
GO

-- Ahora crear el esquema dentro de la base de datos PlaniFy para mas orden
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'PlaniFy')
BEGIN
    EXEC('CREATE SCHEMA PlaniFy');
    PRINT 'Esquema PlaniFy creado correctamente.';
END
ELSE
BEGIN
    PRINT 'El esquema PlaniFy ya existe.';
END
GO