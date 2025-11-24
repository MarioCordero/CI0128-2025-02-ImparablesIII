SELECT * FROM PlaniFy.Persona WHERE Rol = 'Empleador';

SELECT * FROM PlaniFy.Direccion;

DELETE FROM PlaniFy.Direccion WHERE Id BETWEEN 53 AND 61;

SELECT * FROM PlaniFy.Persona;
DELETE FROM PlaniFy.Persona WHERE Id = 74;

SELECT * FROM PlaniFy.Usuario;

SELECT * FROM PlaniFy.Empleado;

SELECT * FROM PlaniFy.Empresa;

SELECT * FROM PlaniFy.Beneficio


DECLARE @CompanyId INT = 6;

SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor as Value, Porcentaje as Percentage, Descripcion
FROM PlaniFy.Beneficio
WHERE idEmpresa = @CompanyId
ORDER BY Nombre;

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmpleadorProyecto';
SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmpleadorProyecto';

SELECT * FROM PlaniFy.Empresa

DECLARE @EmployerId INT = 25;
SELECT 
    e.Id,
    e.Nombre,
    e.CedulaJuridica,
    e.Email,
    e.PeriodoPago,
    e.Telefono,
    e.MaximoBeneficios,
    e.idEmpleador
FROM PlaniFy.Empresa AS e
WHERE e.idEmpleador = @EmployerId;



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