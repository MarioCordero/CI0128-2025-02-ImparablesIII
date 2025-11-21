-- ===================================
-- (18) Add Employee Deletion Fields
-- ===================================
-- Adds fields for logical deletion

-- Add Estado column (Activo/Inactivo)
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('PlaniFy.Empleado') 
    AND name = 'Estado'
)
BEGIN
    ALTER TABLE PlaniFy.Empleado
    ADD Estado NVARCHAR(20) DEFAULT N'Activo' NOT NULL;
    
    -- Add check constraint
    ALTER TABLE PlaniFy.Empleado
    ADD CONSTRAINT CK_Empleado_Estado CHECK (
        Estado IN (N'Activo', N'Inactivo')
    );
    
    -- Update existing records to 'Activo'
    UPDATE PlaniFy.Empleado
    SET Estado = N'Activo'
    WHERE Estado IS NULL;
END;
GO

-- Add FechaBaja column
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('PlaniFy.Empleado') 
    AND name = 'FechaBaja'
)
BEGIN
    ALTER TABLE PlaniFy.Empleado
    ADD FechaBaja DATETIME NULL;
END;
GO

-- Add MotivoBaja column
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('PlaniFy.Empleado') 
    AND name = 'MotivoBaja'
)
BEGIN
    ALTER TABLE PlaniFy.Empleado
    ADD MotivoBaja NVARCHAR(500) NULL;
END;
GO

-- Add UsuarioBajaId column
IF NOT EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('PlaniFy.Empleado') 
    AND name = 'UsuarioBajaId'
)
BEGIN
    ALTER TABLE PlaniFy.Empleado
    ADD UsuarioBajaId INT NULL;
    
    -- Add foreign key constraint
    ALTER TABLE PlaniFy.Empleado
    ADD CONSTRAINT FK_Empleado_UsuarioBaja 
    FOREIGN KEY (UsuarioBajaId) REFERENCES PlaniFy.Persona(Id);
END;
GO

-- Create index for filtering active employees
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Empleado_Estado' 
    AND object_id = OBJECT_ID('PlaniFy.Empleado')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Empleado_Estado
    ON PlaniFy.Empleado (Estado, idEmpresa)
    INCLUDE (idPersona);
END;
GO

