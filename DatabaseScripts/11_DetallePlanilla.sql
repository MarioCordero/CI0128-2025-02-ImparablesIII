-- ===================================
-- (11) TABLA DetallePlanilla (Diego)
-- ===================================
CREATE TABLE PlaniFy.DetallePlanilla (
    idEmpleado INT,
    idPlanilla INT,
    salarioBruto INT,
    totalBeneficios INT,
    DeduccionesEmpleado INT,              -- AÑADIDO: Faltaba
    salarioNeto INT,                      -- AÑADIDO: Faltaba
    DeduccionesEmpresa INT,               -- AÑADIDO: Faltaba
    PRIMARY KEY (idEmpleado, idPlanilla),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id)
);