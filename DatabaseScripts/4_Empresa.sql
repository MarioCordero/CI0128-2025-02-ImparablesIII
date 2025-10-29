-- ===================================
-- (4) TABLA Empresa (Chris)
-- ===================================
CREATE TABLE PlaniFy.Empresa (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    CedulaJuridica INT NOT NULL,
    Email NVARCHAR(50) NOT NULL,
    PeriodoPago NVARCHAR(20) NOT NULL,
    Telefono INT,
    idDireccion INT NOT NULL,
    MaximoBeneficios INT NOT NULL,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (
            N'Mensual',
            N'Quincenal'
        )
    )
);