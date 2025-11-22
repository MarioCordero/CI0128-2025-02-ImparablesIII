SELECT * FROM PlaniFy.Persona WHERE Rol = 'Empleador';

SELECT * FROM PlaniFy.Persona;

SELECT * FROM PlaniFy.Empleado


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
    e.MaximoBeneficios
FROM PlaniFy.Empresa e
INNER JOIN PlaniFy.EmpleadorProyecto ep ON e.Id = ep.idProyecto
WHERE ep.idEmpleador = @EmployerId;