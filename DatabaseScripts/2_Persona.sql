-- ===================================
-- (2) TABLA Persona (Chris)
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(100) NOT NULL, -- CORREGIDO: 50 → 100
    Nombre NVARCHAR(40) NOT NULL,  -- CORREGIDO: 20 → 40
    SegundoNombre NVARCHAR(40),    -- CORREGIDO: 20 → 40
    Apellidos NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(40),              -- CORREGIDO: 20 → 40, NULLABLE
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);
