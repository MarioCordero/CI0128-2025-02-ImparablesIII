-- ===================================
-- (1) TABLA Dirección (Chris)
-- ===================================
CREATE TABLE PlaniFy.Direccion (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    Provincia NVARCHAR(12),
    Canton NVARCHAR(30),
    Distrito NVARCHAR(30),
    DireccionParticular NVARCHAR(150),
    CONSTRAINT CK_Direccion_Provincia CHECK (
        Provincia IN (
            N'San José',
            N'Alajuela',
            N'Cartago',
            N'Heredia',
            N'Guanacaste',
            N'Puntarenas',
            N'Limón'
        )
    )
);