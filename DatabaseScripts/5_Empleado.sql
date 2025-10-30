-- ===================================
-- (5) TABLA Empleado (Chino)
-- ===================================
CREATE TABLE PlaniFy.Empleado (
    idPersona INT PRIMARY KEY NOT NULL,
    Departamento NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    TipoContrato NVARCHAR(50) NOT NULL, -- CORREGIDO: 25 → 50
    TipoSalario NVARCHAR(20),           -- CORREGIDO: 10 → 20
    Puesto NVARCHAR(40) NOT NULL,       -- CORREGIDO: 20 → 40
    FechaContratacion DATE NOT NULL,
    Salario INT NOT NULL,
    iban NVARCHAR(60) NOT NULL,         -- CORREGIDO: 30 → 60
    Contrasena NVARCHAR(32),            -- CORREGIDO: 16 → 32
    idEmpresa INT,                      -- CORREGIDO: NOT NULL → NULLABLE
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
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