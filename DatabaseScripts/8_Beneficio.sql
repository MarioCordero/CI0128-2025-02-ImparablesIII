-- ===================================
-- (8) TABLA Beneficio (Chino)
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    TipoCalculo NVARCHAR(20) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
        ON DELETE CASCADE
);