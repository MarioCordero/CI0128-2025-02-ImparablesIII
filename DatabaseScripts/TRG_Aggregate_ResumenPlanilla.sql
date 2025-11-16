-- ===================================
-- (15) TABLA ResumenPlanilla + TRIGGER de agregación
-- Mantiene totales por idPlanilla y idEmpresa al insertar DetallePlanilla
-- ===================================

IF OBJECT_ID('PlaniFy.ResumenPlanilla', 'U') IS NULL
BEGIN
    CREATE TABLE PlaniFy.ResumenPlanilla (
        idPlanilla INT PRIMARY KEY,
        idEmpresa INT NOT NULL,
        TotalSalarioBruto INT NOT NULL,
        TotalDeduccionesEmpleado INT NOT NULL,
        TotalDeduccionesEmpresa INT NOT NULL,
        TotalBeneficios INT NOT NULL,
        TotalSalarioNeto INT NOT NULL,
    );
END;
GO

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TRG_Aggregate_ResumenPlanilla')
    DROP TRIGGER PlaniFy.TRG_Aggregate_ResumenPlanilla;
GO

CREATE TRIGGER PlaniFy.TRG_Aggregate_ResumenPlanilla
ON PlaniFy.DetallePlanilla
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Planillas TABLE (idPlanilla INT);

    INSERT INTO @Planillas (idPlanilla)
    SELECT DISTINCT idPlanilla FROM inserted;

    -- Recalcular resumen por cada planilla afectada SOLO en inserciones
    MERGE PlaniFy.ResumenPlanilla AS target
    USING (
        SELECT 
            p.id AS idPlanilla,
            p.idEmpresa AS idEmpresa,
            SUM(d.salarioBruto) AS TotalSalarioBruto,
            SUM(d.DeduccionesEmpleado) AS TotalDeduccionesEmpleado,
            SUM(d.DeduccionesEmpresa) AS TotalDeduccionesEmpresa,
            SUM(d.totalBeneficios) AS TotalBeneficios,
            SUM(d.salarioNeto) AS TotalSalarioNeto
        FROM PlaniFy.Planilla p
        JOIN PlaniFy.DetallePlanilla d ON d.idPlanilla = p.id
        WHERE p.id IN (SELECT idPlanilla FROM @Planillas)
        GROUP BY p.id, p.idEmpresa
    ) AS src
    ON target.idPlanilla = src.idPlanilla
    WHEN MATCHED THEN UPDATE SET
        target.idEmpresa = src.idEmpresa,
        target.TotalSalarioBruto = src.TotalSalarioBruto,
        target.TotalDeduccionesEmpleado = src.TotalDeduccionesEmpleado,
        target.TotalDeduccionesEmpresa = src.TotalDeduccionesEmpresa,
        target.TotalBeneficios = src.TotalBeneficios,
        target.TotalSalarioNeto = src.TotalSalarioNeto
    WHEN NOT MATCHED BY TARGET THEN INSERT (
        idPlanilla, idEmpresa, TotalSalarioBruto, TotalDeduccionesEmpleado, TotalDeduccionesEmpresa, TotalBeneficios, TotalSalarioNeto
    ) VALUES (
        src.idPlanilla, src.idEmpresa, src.TotalSalarioBruto, src.TotalDeduccionesEmpleado, src.TotalDeduccionesEmpresa, src.TotalBeneficios, src.TotalSalarioNeto
    );
END;
GO

