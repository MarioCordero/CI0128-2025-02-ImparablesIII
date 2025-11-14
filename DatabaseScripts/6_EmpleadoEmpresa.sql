-- ===================================
-- (6) TABLA EmpleadoEmpresa (Chino)
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);