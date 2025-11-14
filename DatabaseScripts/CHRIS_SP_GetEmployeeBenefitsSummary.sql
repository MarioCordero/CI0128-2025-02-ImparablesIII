-- ===================================
-- STORED PROCEDURE: GetEmployeeBenefitsSummary
-- Optimizes multiple database calls into a single comprehensive procedure
-- ===================================

CREATE OR ALTER PROCEDURE PlaniFy.GetEmployeeBenefitsSummary
    @EmployeeId INT,
    @CompanyId INT,
    @SearchTerm NVARCHAR(50) = NULL,
    @CalculationType NVARCHAR(20) = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get company settings and employee counts
    DECLARE @MaxBenefitLimit INT = 3; -- Default value
    DECLARE @TotalEmployees INT = 0;
    DECLARE @CurrentSelections INT = 0;
    
    -- Try to get max benefit limit from company settings
    BEGIN TRY
        SELECT @MaxBenefitLimit = ISNULL(MaximoBeneficios, 3)
        FROM PlaniFy.Empresa
        WHERE Id = @CompanyId;
    END TRY
    BEGIN CATCH
        SET @MaxBenefitLimit = 3; -- Default if column doesn't exist
    END CATCH
    
    -- Get total employee count for the company
    SELECT @TotalEmployees = COUNT(DISTINCT idPersona)
    FROM PlaniFy.Empleado
    WHERE IdEmpresa = @CompanyId;
    
    -- Get current selections count for the employee
    SELECT @CurrentSelections = COUNT(1)
    FROM PlaniFy.BeneficioEmpleado
    WHERE idEmpleado = @EmployeeId AND idEmpresa = @CompanyId;
    
    -- Main query to get all benefits with their status and usage statistics
    WITH BenefitUsageStats AS (
        SELECT 
            b.idEmpresa,
            b.Nombre,
            COUNT(DISTINCT be.idEmpleado) as EmployeeCount
        FROM PlaniFy.Beneficio b
        LEFT JOIN PlaniFy.BeneficioEmpleado be ON b.idEmpresa = be.idEmpresa 
            AND b.Nombre = be.NombreBeneficio
        WHERE b.idEmpresa = @CompanyId
        GROUP BY b.idEmpresa, b.Nombre
    )
    SELECT 
        b.idEmpresa as CompanyId,
        b.Nombre as BenefitName,
        b.TipoCalculo as CalculationType,
        b.Tipo as BenefitType,
        CASE 
            WHEN b.TipoCalculo = 'Porcentaje' THEN NULL 
            ELSE b.Valor 
        END as Value,
        CASE 
            WHEN b.TipoCalculo = 'Porcentaje' THEN b.Porcentaje 
            ELSE NULL 
        END as Percentage,
        CASE WHEN be_selected.idEmpleado IS NOT NULL THEN 1 ELSE 0 END as IsSelected,
        bus.EmployeeCount,
        CASE 
            WHEN @TotalEmployees > 0 THEN (bus.EmployeeCount * 100.0) / @TotalEmployees 
            ELSE 0 
        END as UsagePercentage,
        @CurrentSelections as CurrentSelections,
        @MaxBenefitLimit as MaxSelections,
        CASE 
            WHEN be_selected.idEmpleado IS NOT NULL THEN 'Seleccionado' 
            ELSE 'Disponible' 
        END as Status,
        -- Employee-specific fields from BeneficioEmpleado table
        be_selected.NumeroDependientes as NumDependents,
        be_selected.TipoPension as PensionType
    FROM PlaniFy.Beneficio b
    LEFT JOIN PlaniFy.BeneficioEmpleado be_selected ON b.idEmpresa = be_selected.idEmpresa 
        AND b.Nombre = be_selected.NombreBeneficio 
        AND be_selected.idEmpleado = @EmployeeId
    LEFT JOIN BenefitUsageStats bus ON b.idEmpresa = bus.idEmpresa 
        AND b.Nombre = bus.Nombre
    WHERE b.idEmpresa = @CompanyId
        AND (@SearchTerm IS NULL OR b.Nombre LIKE '%' + @SearchTerm + '%')
        AND (@CalculationType IS NULL OR b.TipoCalculo = @CalculationType)
        AND (@Status IS NULL OR 
             (@Status = 'Seleccionado' AND be_selected.idEmpleado IS NOT NULL) OR
             (@Status = 'Disponible' AND be_selected.idEmpleado IS NULL))
    ORDER BY b.Nombre;
    
    -- Return summary information as a separate result set
    SELECT 
        @CurrentSelections as CurrentSelections,
        @MaxBenefitLimit as MaxSelections,
        @TotalEmployees as TotalEmployees,
        CASE WHEN @CurrentSelections < @MaxBenefitLimit THEN 1 ELSE 0 END as CanSelectMore;
END;
