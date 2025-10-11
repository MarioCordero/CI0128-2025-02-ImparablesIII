-- ===================================
-- (6) TABLA EmpleadoEmpresa (Chino)
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE, -- Si se borra el empleado, se borra la relacion
    -- VIEJO: FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE -- Si se borra la empresa, se borran las relaciones
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE NO ACTION -- Cambiar a NO ACTION para evitar ciclo de los ON CASCADE
);