-- ===================================
-- (10) TABLA HorasTrabajadas (Mario)
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    -- id INT IDENTITY PRIMARY KEY NOT NULL,
    id INT IDENTITY NOT NULL,
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(150) NOT NULL,
    Estado BIT DEFAULT 0,
    idAprobador INT NOT NULL,
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    -- FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Empleado(idPersona) -- SEGUN EL MODELO RELACIONAL EL ID VIENE REFERENCIADO DE PERSONA, NO DE EMPLEADO
    -- CAMBIOS
    PRIMARY KEY (id, idEmpleado),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id)
);