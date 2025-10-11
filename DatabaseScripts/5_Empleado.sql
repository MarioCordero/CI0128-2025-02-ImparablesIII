-- ===================================
-- (5) TABLA Empleado (Chino)
-- ===================================
CREATE TABLE PlaniFy.Empleado (
    idPersona INT PRIMARY KEY NOT NULL,
    Departamento NVARCHAR(20) NOT NULL,
    TipoContrato NVARCHAR(25) NOT NULL,
    TipoSalario NVARCHAR(10),
    Puesto NVARCHAR(20) NOT NULL,
    FechaContratacion DATE NOT NULL,
    Salario INT NOT NULL,
    iban NVARCHAR(30) NOT NULL,
    Contrasena NVARCHAR(16),
    idEmpresa INT NOT NULL,
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE, -- Si se borra la persona, se borra el empleado
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE, -- Si se borra la empresa, se borran los empleados
    CONSTRAINT CK_Tipo_Contrato CHECK (
        TipoContrato IN (
            N'Tiempo Completo',
            N'Medio Tiempo',
            N'Servicios Profesionales'
        )
    ),
    CONSTRAINT CK_Tipo_Salario CHECK (
        TipoSalario IN (
            N'Mensual',
            N'Quincenal'
        )
    )
);