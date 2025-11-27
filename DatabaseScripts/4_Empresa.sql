-- ===================================
-- (4) TABLA Empresa (Chris)
-- ===================================
CREATE TABLE PlaniFy.Empresa (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Nombre NVARCHAR(40) NOT NULL,      -- CORREGIDO: 20 → 40
    CedulaJuridica INT NOT NULL,
    Email NVARCHAR(100) NOT NULL,      -- CORREGIDO: 50 → 100
    PeriodoPago NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    Telefono INT,
    idDireccion INT NOT NULL,
    MaximoBeneficios INT,              -- AÑADIDO: Falta en tu script
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (
            N'Mensual',
            N'Quincenal'
        )
    )
);

-- MODIFICACIONES PARA RELACIONAR EMPRESA CON EMPLEADOR (Mario)

ALTER TABLE PlaniFy.Empresa
ADD idEmpleador INT NULL;

ALTER TABLE PlaniFy.Empresa
ADD CONSTRAINT FK_Empresa_Empleador
FOREIGN KEY (idEmpleador)
REFERENCES PlaniFy.Persona(Id);

-- MODIFICACIONES PARA ELIMINAR EMPRESA (Mario)

ALTER TABLE PlaniFy.Empresa
ADD FechaBaja DATETIME NULL,
    MotivoBaja NVARCHAR(100) NULL,
    UsuarioBajaId INT NULL;
    
ALTER TABLE PlaniFy.Empresa
ADD CONSTRAINT FK_Empresa_UsuarioBaja
FOREIGN KEY (UsuarioBajaId) REFERENCES PlaniFy.Persona(Id);

ALTER TABLE PlaniFy.Empresa
ADD Estado NVARCHAR(10) NULL,
    CONSTRAINT CK_Empresa_Estado CHECK (
        Estado IN (
            N'Activo',
            N'Inactivo'
        ) OR Estado IS NULL
    );