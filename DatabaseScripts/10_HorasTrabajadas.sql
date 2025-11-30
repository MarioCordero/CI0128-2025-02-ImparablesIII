-- ===================================
-- (10) TABLA HorasTrabajadas (Mario)
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(300) NOT NULL,
    Fecha DATE NOT NULL,
    Estado VARCHAR(9) NOT NULL DEFAULT 'Pendiente',
    idAprobador INT NOT NULL,

    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id),
    CONSTRAINT CK_HorasTrabajadas_Estado CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado'))
);