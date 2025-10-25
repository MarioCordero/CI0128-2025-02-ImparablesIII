-- ===================================
-- (2) TABLA Persona (Chris)
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    SegundoNombre NVARCHAR(20),
    Apellidos NVARCHAR(20) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(20) NOT NULL,
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);