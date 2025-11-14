-- ===================================
-- Add Puesto column to DetallePlanilla for historical position tracking
-- ===================================

-- Add Puesto column to store the position at the time of payroll generation
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('PlaniFy.DetallePlanilla') 
    AND name = 'Puesto'
)
BEGIN
    ALTER TABLE PlaniFy.DetallePlanilla
    ADD Puesto NVARCHAR(20) NULL;
    
    -- Update existing records with current position from Empleado table
    UPDATE dp
    SET dp.Puesto = e.Puesto
    FROM PlaniFy.DetallePlanilla dp
    INNER JOIN PlaniFy.Empleado e ON e.idPersona = dp.idEmpleado
    WHERE dp.Puesto IS NULL;
    
    -- Make it NOT NULL after backfilling
    ALTER TABLE PlaniFy.DetallePlanilla
    ALTER COLUMN Puesto NVARCHAR(20) NOT NULL;
END;
GO

