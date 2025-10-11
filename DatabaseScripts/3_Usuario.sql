-- ===================================
-- (3) TABLA Usuario (Chino)
-- ===================================
CREATE TABLE PlaniFy.Usuario (
    idPersona INT PRIMARY KEY NOT NULL,
    TipoUsuario NVARCHAR(20) NOT NULL,
    Contrasena NVARCHAR(16) NOT NULL,
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) 
        ON DELETE CASCADE, -- Si se borra la persona, se borra el usuario
    CONSTRAINT CK_Tipo_Usuario CHECK (
        TipoUsuario IN (
            N'Administrador',
            N'Empleador',
            N'Empleado',
            N'RH'
        )
    )
);