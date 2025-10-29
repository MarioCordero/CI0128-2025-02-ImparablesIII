-- ===================================
-- (11) TABLA DetallePlanilla (Diego)
-- ===================================
CREATE TABLE PlaniFy.DetallePlanilla (
    idEmpleado INT,
    idPlanilla INT,
    salarioBruto INT,
    DeduccionesEmpleado INT,
    DeduccionesEmpresa INT,
    totalBeneficios INT,
    salarioNeto INT,
    PRIMARY KEY (idEmpleado, idPlanilla),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id)
);