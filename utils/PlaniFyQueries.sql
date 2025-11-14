SELECT * FROM PlaniFy.Persona WHERE Rol = 'Empleador';

SELECT * FROM PlaniFy.Persona;

SELECT * FROM PlaniFy.Empleado


DECLARE @CompanyId INT = 6;

SELECT idEmpresa as CompanyId, Nombre as Name, TipoCalculo as CalculationType, Tipo as Type, Valor as Value, Porcentaje as Percentage, Descripcion
FROM PlaniFy.Beneficio
WHERE idEmpresa = @CompanyId
ORDER BY Nombre;