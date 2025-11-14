-- ===================================
-- (10) TABLA HorasTrabajadas (Mario)
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL, -- CORREGIDO: PK simple (no compuesta)
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(300) NOT NULL,       -- CORREGIDO: 150 → 300
    Estado BIT DEFAULT 0,
    idAprobador INT NOT NULL,
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id)
);