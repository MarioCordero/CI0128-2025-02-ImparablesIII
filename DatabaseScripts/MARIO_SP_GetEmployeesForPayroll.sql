CREATE PROCEDURE PlaniFy.GetEmployeesForPayroll
    @CompanyId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        emp.idPersona AS IdEmpleado,
        emp.idEmpresa AS IdEmpresa,
        p.Nombre AS Nombre,
        p.Apellidos AS Apellidos,
        emp.Salario AS SalarioBruto,
        emp.FechaContratacion AS FechaIngreso,
        e.PeriodoPago AS TipoPago -- 'Quincenal' o 'Mensual'
    FROM PlaniFy.Empleado emp
    INNER JOIN PlaniFy.Persona p ON emp.idPersona = p.Id
    INNER JOIN PlaniFy.Empresa e ON emp.idEmpresa = e.Id
    WHERE emp.idEmpresa = @CompanyId
    FOR JSON PATH, ROOT('Empleados')
END

-- Prueba
-- EXEC PlaniFy.GetEmployeesForPayroll @CompanyId = 6;