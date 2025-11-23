-- ===================================
-- (10) TABLA HorasTrabajadas (Mario)
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL, -- CORREGIDO: PK simple (no compuesta)
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(300) NOT NULL,       -- CORREGIDO: 150 → 300
    Fecha DATE NOT NULL,
    Estado VARCHAR(9) NOT NULL DEFAULT 'Pendiente', -- CORREGIDO: BIT → VARCHAR(9) con default 'Pendiente'
    idAprobador INT NOT NULL,

    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id),
    CONSTRAINT CK_HorasTrabajadas_Estado CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado'))
);