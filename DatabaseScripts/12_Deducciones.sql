-- ===================================
-- (12) TABLA Deducciones (Diego)
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(40) NOT NULL,         -- CORREGIDO: 20 → 40
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(40),               -- CORREGIDO: 20 → 40
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    FOREIGN KEY (idEmpresa, Beneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE SET NULL
);