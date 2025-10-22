-- ===================================
-- (12) TABLA Deducciones (Diego)
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(20),
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(20),
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    -- FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) -- SEGUN EL MODELO RELACIONAL EL ID VIENE REFERENCIADO DE BENEFICIO, NO DE EMPRESA
    -- NUEVO
    FOREIGN KEY (idEmpresa, Beneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE SET NULL
);