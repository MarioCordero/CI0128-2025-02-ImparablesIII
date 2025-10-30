-- ===================================
-- (1) TABLA Dirección (Chris)
-- ===================================
CREATE TABLE PlaniFy.Direccion (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    Provincia NVARCHAR(24), -- CORREGIDO: 12 → 24
    Canton NVARCHAR(60),    -- CORREGIDO: 30 → 60
    Distrito NVARCHAR(60),  -- CORREGIDO: 30 → 60
    DireccionParticular NVARCHAR(300), -- CORREGIDO: 150 → 300
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