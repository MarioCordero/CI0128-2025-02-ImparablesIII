-- ===================================
-- (9) TABLA BeneficioEmpleado (Mario)
-- ===================================
CREATE TABLE PlaniFy.BeneficioEmpleado (
    idEmpleado INT NOT NULL,
    NombreBeneficio NVARCHAR(100) NOT NULL, -- CORREGIDO: 20 → 100
    idEmpresa INT NOT NULL,
    TipoBeneficio NVARCHAR(40) NOT NULL,    -- CORREGIDO: 20 → 40
    NumeroDependientes INT,                 -- AÑADIDO: Falta en tu script
    TipoPension NVARCHAR(2),                -- AÑADIDO: Falta en tu script
    PRIMARY KEY (idEmpleado, NombreBeneficio, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
    FOREIGN KEY (idEmpresa, NombreBeneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre) ON DELETE CASCADE,
    CONSTRAINT CHK_BeneficioEmpleado_NumeroDependientes CHECK (NumeroDependientes IS NULL OR NumeroDependientes >= 0),
    CONSTRAINT CHK_BeneficioEmpleado_TipoPension CHECK (TipoPension IS NULL OR (TipoPension='C' OR TipoPension='B' OR TipoPension='A'))
);