-- ===================================
-- (15) TABLA ResumenPlanilla
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