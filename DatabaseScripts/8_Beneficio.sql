-- ===================================
-- (8) TABLA Beneficio (Chino)
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,      -- CORREGIDO: 20 → 100
    TipoCalculo NVARCHAR(40) NOT NULL,  -- CORREGIDO: 20 → 40
    Tipo NVARCHAR(40) NOT NULL,         -- CORREGIDO: 20 → 40
    Valor INT,
    Porcentaje INT,
    Descripcion VARCHAR(200),           -- AÑADIDO: Faltaba
    IsDeleted BIT DEFAULT 0,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE
);