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
