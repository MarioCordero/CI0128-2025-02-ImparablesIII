CREATE TABLE PlaniFy.EmployeeDeductions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(32) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2) NULL,
    MaxAmount DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Tabla de deducciones de empleador
CREATE TABLE PlaniFy.EmployerDeductions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(32) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2) NULL,
    MaxAmount DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Inserción de valores para EmployeeDeductions
INSERT INTO PlaniFy.EmployeeDeductions (Code, Name, Rate, MinAmount, MaxAmount, IsActive) VALUES
('CCSS_SEM_EE', 'CCSS Enfermedad-Maternidad', 0.055, 0, NULL, 1),
('CCSS_IVM_EE', 'CCSS IVM', 0.0417, 0, NULL, 1),
('BP_TRAB', 'Banco Popular Trabajador', 0.01, 0, NULL, 1),
('RENTA', 'Impuesto de Salario', 0.10, 922000, 1352000, 1),
('RENTA', 'Impuesto de Salario', 0.15, 1352000, 2373000, 1),
('RENTA', 'Impuesto de Salario', 0.20, 2373000, 4750000, 1),
('RENTA', 'Impuesto de Salario', 0.25, 4750000, NULL, 1);

-- Inserción de valores para EmployerDeductions
INSERT INTO PlaniFy.EmployerDeductions (Code, Name, Rate, MinAmount, MaxAmount, IsActive) VALUES
('CCSS_SEM_ER', 'CCSS Enfermedad-Maternidad', 0.0775, 0, NULL, 1),
('CCSS_IVM_ER', 'CCSS IVM', 0.0525, 0, NULL, 1),
('BP_PATRON', 'Banco Popular Patronal', 0.0033, 0, NULL, 1),
('CCSS_SEM_ER', 'CCSS Enfermedad-Maternidad', 0.0775, 0, NULL, 1),
('CCSS_IVM_ER', 'CCSS IVM', 0.0525, 0, NULL, 1),
('BP_PATRON', 'Banco Popular Patronal', 0.0033, 0, NULL, 1),
('INA_PATR', 'Instituto Nacional de Aprendizaje', 0.015, 0, NULL, 1),
('FCL_PATR', 'Fondo de Capitalización Laboral', 0.0333, 0, NULL, 1),
('FODESAF_PATR', 'FODESAF', 0.05, 0, NULL, 1),
('FPC_PATR', 'Fondo de Pensiones Complementarias', 0.0275, 0, NULL, 1);

SELECT * FROM PlaniFy.EmployeeDeductions;
SELECT * FROM PlaniFy.EmployerDeductions;