-- ===================================
-- (3) TABLA Usuario (Chino)
-- ===================================
CREATE TABLE PlaniFy.Usuario (
    idPersona INT PRIMARY KEY NOT NULL,
    TipoUsuario NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    Contrasena NVARCHAR(32) NOT NULL,  -- CORREGIDO: 16 → 32
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Tipo_Usuario CHECK (
        TipoUsuario IN (
            N'Administrador',
            N'Empleador',
            N'Empleado',
            N'RH'
        )
    )
);

-- Índice para optimizar búsquedas por TipoUsuario
-- TODO: Probar eficiencia
ALTER TABLE PlaniFy.Usuario
    ADD VerificationTokenHash NVARCHAR(64) NULL,
        VerificationTokenExpires DATETIME2 NULL,
        IsVerified BIT NOT NULL DEFAULT 0;
GO
CREATE INDEX IX_Usuario_VerificationTokenHash ON PlaniFy.Usuario (VerificationTokenHash);