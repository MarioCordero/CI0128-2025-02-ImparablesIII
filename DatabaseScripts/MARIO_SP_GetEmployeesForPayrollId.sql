CREATE PROCEDURE PlaniFy.GetEmployeesForPayrollId
    @PayrollId INT
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
        e.PeriodoPago AS TipoPago
    FROM PlaniFy.DetallePlanilla dp
    INNER JOIN PlaniFy.Empleado emp ON dp.idEmpleado = emp.idPersona
    INNER JOIN PlaniFy.Persona p ON emp.idPersona = p.Id
    INNER JOIN PlaniFy.Empresa e ON emp.idEmpresa = e.Id
    WHERE dp.idPlanilla = @PayrollId
    FOR JSON PATH, ROOT('Empleados')
END

-- Prueba
-- EXEC PlaniFy.GetEmployeesForPayrollId @PayrollId = 19;