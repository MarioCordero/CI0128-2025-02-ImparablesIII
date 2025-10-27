CREATE OR ALTER PROCEDURE PlaniFy.sp_PayrollReport
  @CompanyId   INT,
  @Year        INT,
  @Month       INT,
  @PeriodType  NVARCHAR(10),   -- 'Monthly' | 'Biweekly'
  @Fortnight   TINYINT = NULL, -- 1 or 2 (Biweekly)
  @Department  NVARCHAR(20) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @From DATE, @To DATE;

  IF (@PeriodType = N'Monthly')
  BEGIN
    SET @From = DATEFROMPARTS(@Year,@Month,1);
    SET @To   = EOMONTH(@From);
  END
  ELSE
  BEGIN
    DECLARE @Q TINYINT = CASE WHEN @Fortnight IN (1,2) THEN @Fortnight ELSE 1 END;
    SET @From = CASE WHEN @Q=1 THEN DATEFROMPARTS(@Year,@Month,1)
                     ELSE DATEFROMPARTS(@Year,@Month,16) END;
    SET @To   = CASE WHEN @Q=1 THEN DATEFROMPARTS(@Year,@Month,15)
                     ELSE EOMONTH(DATEFROMPARTS(@Year,@Month,1)) END;
  END

  ;WITH Emps AS (
    SELECT
      e.idPersona AS EmployeeId,
      p.Nombre + N' ' + p.Apellidos AS FullName,
      e.Departamento,
      CAST(CASE WHEN @PeriodType=N'Biweekly' THEN e.Salario/2.0 ELSE e.Salario END AS DECIMAL(18,2)) AS Bruto
    FROM PlaniFy.Empleado e
    JOIN PlaniFy.Persona p ON p.Id = e.idPersona
    WHERE e.idEmpresa = @CompanyId
      AND (@Department IS NULL OR e.Departamento = @Department)
  ),
  Pay AS (
    SELECT Id, FechaGeneracion, Estado, FechaPago
    FROM PlaniFy.Planilla
    WHERE idEmpresa = @CompanyId
      AND CAST(FechaGeneracion AS DATE) BETWEEN @From AND @To
  ),
  Ded AS (
    SELECT d.idEmpleado,
           SUM(CASE WHEN d.Tipo = N'EE' THEN d.Monto ELSE 0 END) AS EE,
           SUM(CASE WHEN d.Tipo = N'ER' THEN d.Monto ELSE 0 END) AS ER
    FROM PlaniFy.DeduccionEmpleado d
    WHERE d.idPlanilla IN (SELECT Id FROM Pay)
    GROUP BY d.idEmpleado
  ),
  BenCalc AS (
    SELECT be.idEmpleado,
           CASE
             WHEN b.TipoCalculo = N'Porcentaje' AND b.Porcentaje IS NOT NULL
               THEN CAST(e.Bruto * (b.Porcentaje/100.0) AS DECIMAL(18,2))
             ELSE CAST(ISNULL(b.Valor,0) AS DECIMAL(18,2))
           END AS Monto
    FROM PlaniFy.BeneficioEmpleado be
    JOIN PlaniFy.Beneficio b
      ON b.idEmpresa = be.idEmpresa AND b.Nombre = be.NombreBeneficio
    JOIN Emps e ON e.EmployeeId = be.idEmpleado
    WHERE be.idEmpresa = @CompanyId
  ),
  Ben AS (
    SELECT idEmpleado, SUM(Monto) AS Beneficios
    FROM BenCalc
    GROUP BY idEmpleado
  ),
  PayStatus AS (
    SELECT dp.idEmpleado,
           CASE WHEN MAX(CASE WHEN (p.Estado = N'Pagado' OR p.FechaPago IS NOT NULL) THEN 1 ELSE 0 END) = 1
                THEN 1 ELSE 0 END AS Paid,
           MAX(p.FechaPago) AS PaidAt
    FROM PlaniFy.DetallePlanilla dp
    JOIN Pay p ON p.Id = dp.idPlanilla
    GROUP BY dp.idEmpleado
  )
  -- Result set #1: employee rows
  SELECT
    e.EmployeeId,
    e.FullName,
    e.Departamento AS Department,
    e.Bruto AS GrossSalary,
    ISNULL(d.EE,0) AS EmployeeDeductions,
    ISNULL(d.ER,0) AS EmployerDeductions,
    ISNULL(b.Beneficios,0) AS Benefits,
    e.Bruto - ISNULL(d.EE,0) + ISNULL(b.Beneficios,0) AS NetSalary,
    CASE WHEN ps.Paid = 1 THEN N'Paid' ELSE N'Pending' END AS PaymentStatus,
    ps.PaidAt
  FROM Emps e
  LEFT JOIN Ded d ON d.idEmpleado = e.EmployeeId
  LEFT JOIN Ben b ON b.idEmpleado = e.EmployeeId
  LEFT JOIN PayStatus ps ON ps.idEmpleado = e.EmployeeId
  ORDER BY e.FullName;

  -- Result set #2: company totals
  SELECT
    SUM(e.Bruto) AS TotalGross,
    SUM(ISNULL(d.EE,0)) AS TotalEmployeeDeductions,
    SUM(ISNULL(d.ER,0)) AS TotalEmployerDeductions,
    SUM(ISNULL(b.Beneficios,0)) AS TotalBenefits,
    SUM(e.Bruto - ISNULL(d.EE,0) + ISNULL(b.Beneficios,0)) AS TotalNet,
    COUNT(*) AS EmployeeCount
  FROM Emps e
  LEFT JOIN Ded d ON d.idEmpleado = e.EmployeeId
  LEFT JOIN Ben b ON b.idEmpleado = e.EmployeeId;
END
GO