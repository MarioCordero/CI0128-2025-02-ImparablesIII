SELECT * FROM PlaniFy.Direccion;
DELETE FROM PlaniFy.Direccion WHERE Id = 103;

SELECT COUNT(*) FROM PlaniFy.Direccion;

SELECT * FROM PlaniFy.Persona;
DELETE FROM PlaniFy.Persona WHERE Id = 93;

SELECT * FROM PlaniFy.Usuario;

-- Elimina direcciones que no están asociadas ni a Persona ni a Empresa
DELETE FROM PlaniFy.Direccion
WHERE Id NOT IN (SELECT idDireccion FROM PlaniFy.Persona)
  AND Id NOT IN (SELECT idDireccion FROM PlaniFy.Empresa);

UPDATE PlaniFy.Usuario
SET Contrasena = '$2a$11$PHLDj2NC6lmE2qnsrNyl2OrbRipaC/f17tPKFTqNfmxmXWsKKnfJ.', VerificationTokenHash = NULL, VerificationTokenExpires = NULL, IsVerified = 1


SELECT * FROM PlaniFy.Empresa;
SELECT * FROM PlaniFy.Empleado;
SELECT * FROM PlaniFy.Planilla;
SELECT * FROM PlaniFy.DetallePlanilla;

SELECT * FROM PlaniFy.EmployeeDeductions
SELECT * FROM PlaniFy.EmployerDeductions


SELECT * FROM PlaniFy.Beneficio


DECLARE @CompanyId INT = 6;

SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor as Value, Porcentaje as Percentage, Descripcion
FROM PlaniFy.Beneficio
WHERE idEmpresa = @CompanyId
ORDER BY Nombre;

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmpleadorProyecto';
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmpleadorProyecto';

SELECT * FROM PlaniFy.Empresa

SELECT 
    e.idPersona AS Id,
    CONCAT(p.Nombre, ' ', COALESCE(p.SegundoNombre + ' ', ''), p.Apellidos) AS NombreCompleto,
    p.Correo,
    p.Telefono,
    e.Puesto,
    e.Departamento,
    e.Salario,
    e.TipoContrato
FROM PlaniFy.Empleado e
INNER JOIN PlaniFy.Persona p ON p.Id = e.idPersona
WHERE e.idEmpresa = 17
AND (e.Estado = 'Activo' OR e.Estado IS NULL);



-- BORRAR TODA LA BASE DE DATOS  
-- Deshabilitar restricciones de claves foráneas
ALTER TABLE PlaniFy.ResumenPlanilla NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Deducciones NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.DetallePlanilla NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.HorasTrabajadas NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.BeneficioEmpleado NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Beneficio NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Planilla NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.EmpleadoEmpresa NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Empleado NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Empresa NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Usuario NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Persona NOCHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Direccion NOCHECK CONSTRAINT ALL;

-- Limpiar tablas con DELETE (funciona mejor con FK)
DELETE FROM PlaniFy.ResumenPlanilla;
DELETE FROM PlaniFy.DetallePlanilla;
DELETE FROM PlaniFy.HorasTrabajadas;
DELETE FROM PlaniFy.BeneficioEmpleado;
DELETE FROM PlaniFy.Deducciones;
DELETE FROM PlaniFy.EmpleadoEmpresa;
DELETE FROM PlaniFy.Beneficio;
DELETE FROM PlaniFy.Planilla;
DELETE FROM PlaniFy.Empleado;
DELETE FROM PlaniFy.Empresa;
DELETE FROM PlaniFy.Usuario;
DELETE FROM PlaniFy.Persona;
DELETE FROM PlaniFy.Direccion;

-- Habilitar restricciones de claves foráneas
ALTER TABLE PlaniFy.Direccion CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Persona CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Usuario CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Empresa CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Empleado CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.EmpleadoEmpresa CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Planilla CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Beneficio CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.BeneficioEmpleado CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.HorasTrabajadas CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.DetallePlanilla CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.Deducciones CHECK CONSTRAINT ALL;
ALTER TABLE PlaniFy.ResumenPlanilla CHECK CONSTRAINT ALL;

SELECT 
    sch.name AS 'Esquema',
    t.name AS 'Tabla',
    ind.name AS 'Nombre_Indice',
    ind.type_desc AS 'Tipo_Indice',
    COL_NAME(ic.object_id, ic.column_id) AS 'Columna',
    ic.is_included_column AS 'Es_Include'
FROM 
    sys.indexes ind 
INNER JOIN 
    sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
    sys.tables t ON ind.object_id = t.object_id 
INNER JOIN 
    sys.schemas sch ON t.schema_id = sch.schema_id
WHERE 
    ind.is_primary_key = 0 -- Cambia a 1 si quieres ver solo las Primary Keys
    AND ind.is_unique_constraint = 0 -- Oculta constraints únicos automáticos
    AND t.is_ms_shipped = 0 
ORDER BY 
    t.name, ind.name, ind.index_id, ic.is_included_column;