-- ===================================
-- (16) EXTENDED TABLA BeneficioEmpleado - Employee-Specific Benefit Data
-- ===================================

-- Add employee-specific fields to BeneficioEmpleado table
ALTER TABLE PlaniFy.BeneficioEmpleado 
ADD 
    -- For Seguro Privado: Number of dependents (siblings/family to take care of)
    NumeroDependientes INT NULL,
    
    -- For Pensiones Voluntarias: Type of pension (A, B, or C)
    TipoPension NVARCHAR(1) NULL,
    
    -- Add check constraints to ensure data integrity
    CONSTRAINT CHK_BeneficioEmpleado_NumeroDependientes 
        CHECK (NumeroDependientes IS NULL OR NumeroDependientes >= 0),
    
    CONSTRAINT CHK_BeneficioEmpleado_TipoPension 
        CHECK (TipoPension IS NULL OR TipoPension IN ('A', 'B', 'C'));
