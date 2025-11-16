-- ===================================
-- (14) TABLA EmployerDeductions
-- ===================================
CREATE TABLE PlaniFy.EmployerDeductions (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Code NVARCHAR(64) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    IsActive BIT DEFAULT 1 NOT NULL
);

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