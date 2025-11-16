-- ===================================
-- Indexes for Employee Payroll Reports Performance Optimization
-- ===================================

-- Index 1: Composite index for filtering payroll reports by employee and date range
-- This index optimizes the query: Get payroll reports for a specific employee filtered by year/month
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_DetallePlanilla_Employee_Planilla' 
    AND object_id = OBJECT_ID('PlaniFy.DetallePlanilla')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_DetallePlanilla_Employee_Planilla
    ON PlaniFy.DetallePlanilla (idEmpleado, idPlanilla)
    INCLUDE (salarioBruto, DeduccionesEmpleado, DeduccionesEmpresa, totalBeneficios, salarioNeto, Puesto);
END;
GO

-- Index 2: Index on Planilla for date-based filtering
-- This index optimizes filtering by FechaGeneracion (year/month)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Planilla_Empresa_FechaGeneracion' 
    AND object_id = OBJECT_ID('PlaniFy.Planilla')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Planilla_Empresa_FechaGeneracion
    ON PlaniFy.Planilla (idEmpresa, FechaGeneracion)
    INCLUDE (id, Horas, idResponsable);
END;
GO

-- Index 3: Composite index for filtering by employee, position, and date
-- This index optimizes queries that filter by Puesto (position) along with date ranges
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_DetallePlanilla_Employee_Puesto' 
    AND object_id = OBJECT_ID('PlaniFy.DetallePlanilla')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_DetallePlanilla_Employee_Puesto
    ON PlaniFy.DetallePlanilla (idEmpleado, Puesto)
    INCLUDE (idPlanilla, salarioBruto, salarioNeto);
END;
GO

-- Index 4: Covering index for employee payroll detail lookups
-- This index covers the most common query pattern: get all payroll details for an employee
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_DetallePlanilla_Covering_Employee' 
    AND object_id = OBJECT_ID('PlaniFy.DetallePlanilla')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_DetallePlanilla_Covering_Employee
    ON PlaniFy.DetallePlanilla (idEmpleado)
    INCLUDE (idPlanilla, salarioBruto, DeduccionesEmpleado, DeduccionesEmpresa, totalBeneficios, salarioNeto, Puesto);
END;
GO

