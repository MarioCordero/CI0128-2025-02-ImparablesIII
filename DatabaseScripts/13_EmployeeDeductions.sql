-- ===================================
-- (13) TABLA EmployerDeductions
-- ===================================
CREATE TABLE PlaniFy.EmployeeDeductions (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Code NVARCHAR(64) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    IsActive BIT DEFAULT 1 NOT NULL
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