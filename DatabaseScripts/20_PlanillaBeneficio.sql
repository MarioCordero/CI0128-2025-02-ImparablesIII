-- ===================================
-- (20) Tabla PlanillaBeneficio
-- ===================================
CREATE TABLE PlaniFy.PlanillaBeneficio (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	IdPlanilla INT NOT NULL,
	IdEmpresa INT NOT NULL,
	NombreBeneficio NVARCHAR(50) NOT NULL,
	FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY (IdPlanilla) REFERENCES PlaniFy.Planilla(Id),
	FOREIGN KEY (IdEmpresa, NombreBeneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
);
GO

CREATE INDEX IX_PlanillaBeneficio_PlanillaEmpresa
ON PlaniFy.PlanillaBeneficio (IdPlanilla, IdEmpresa);
GO
