-- ===================================
-- SCRIPT DE BASE DE DATOS PLANIFY
-- ===================================

-- Crear esquema si no existe
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'PlaniFy')
BEGIN
    EXEC('CREATE SCHEMA PlaniFy');
END;
GO

-- ===================================
-- TABLA Dirección
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

-- ===================================
-- TABLA Persona
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    SegundoNombre NVARCHAR(20),
    Apellidos NVARCHAR(20) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(20),
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);

-- ===================================
-- TABLA Usuario
-- ===================================
CREATE TABLE PlaniFy.Usuario (
    idPersona INT PRIMARY KEY NOT NULL,
    TipoUsuario NVARCHAR(20) NOT NULL,
    Contrasena NVARCHAR(16) NOT NULL,
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) 
        ON DELETE CASCADE,
    CONSTRAINT CK_Tipo_Usuario CHECK (
        TipoUsuario IN (
            N'Administrador',
            N'Empleador',
            N'Empleado',
            N'RH'
        )
    )
);

-- ===================================
-- TABLA Empresa
-- ===================================
CREATE TABLE PlaniFy.Empresa (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    CedulaJuridica INT NOT NULL,
    Email NVARCHAR(50) NOT NULL,
    PeriodoPago NVARCHAR(20) NOT NULL,
    Telefono INT,
    idDireccion INT NOT NULL,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (
            N'Mensual',
            N'Quincenal'
        )
    )
);

-- ===================================
-- TABLA Empleado
-- ===================================
CREATE TABLE PlaniFy.Empleado (
    idPersona INT PRIMARY KEY NOT NULL,
    Departamento NVARCHAR(20) NOT NULL,
    TipoContrato NVARCHAR(25) NOT NULL,
    TipoSalario NVARCHAR(10),
    Puesto NVARCHAR(20) NOT NULL,
    FechaContratacion DATE NOT NULL,
    Salario INT NOT NULL,
    iban NVARCHAR(30) NOT NULL,
    Contrasena NVARCHAR(16),
    idEmpresa INT,
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE,
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

-- ===================================
-- TABLA EmpleadoEmpresa
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE
);

-- ===================================
-- TABLA Planilla
-- ===================================
CREATE TABLE PlaniFy.Planilla (
    id INT IDENTITY PRIMARY KEY,
    FechaGeneracion DATETIME,
    Horas INT,
    idResponsable INT,
    idEmpresa INT,
    FOREIGN KEY (idResponsable) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);

-- ===================================
-- TABLA Beneficio
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    TipoCalculo NVARCHAR(20) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
        ON DELETE CASCADE
);

-- ===================================
-- TABLA BeneficioEmpleado
-- ===================================
CREATE TABLE PlaniFy.BeneficioEmpleado (
    idEmpleado INT NOT NULL,
    NombreBeneficio NVARCHAR(20) NOT NULL,
    idEmpresa INT NOT NULL,
    TipoBeneficio NVARCHAR(20) NOT NULL,
    PRIMARY KEY (idEmpleado, NombreBeneficio, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
    FOREIGN KEY (idEmpresa, NombreBeneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE CASCADE
);

-- ===================================
-- TABLA Deducciones
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(20),
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(20),
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);

-- ===================================
-- TABLA HorasTrabajadas
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(150) NOT NULL,
    Estado BIT DEFAULT 0,
    idAprobador INT NOT NULL,
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Empleado(idPersona)
);

-- ===================================
-- TABLA DetallePlanilla
-- ===================================
CREATE TABLE PlaniFy.DetallePlanilla (
    idEmpleado INT,
    idPlanilla INT,
    salarioBruto INT,
    totalBeneficios INT,
    PRIMARY KEY (idEmpleado, idPlanilla),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id)
);

-- ===================================
-- DATOS DE PRUEBA
-- ===================================

-- Inserts en Direccion
INSERT INTO PlaniFy.Direccion (Provincia, Canton, Distrito, DireccionParticular)
VALUES 
(N'San José', N'San José', N'Carmen', N'Avenida Central, Edificio Torre Norte'),
(N'Alajuela', N'Alajuela', N'Alajuela', N'Del Parque Central 200m Norte'),
(N'Cartago', N'Cartago', N'Oriental', N'Frente a la Basílica'),
(N'Heredia', N'Heredia', N'Heredia', N'Universidad Nacional 100m Sur'),
(N'San José', N'Escazú', N'San Rafael', N'Centro Comercial Multiplaza'),
(N'San José', N'Santa Ana', N'Santa Ana', N'Forum Santa Ana');

-- Inserts en Empresa
INSERT INTO PlaniFy.Empresa (Nombre, CedulaJuridica, Email, PeriodoPago, Telefono, idDireccion)
VALUES 
(N'Grupo Zeta', 301200111, N'contacto@grupozeta.com', N'Mensual', 22223333, 4),
(N'TechSolutions', 302500222, N'info@techsolutions.com', N'Quincenal', 22778899, 5),
(N'Alimentos Tica', 303800333, N'ventas@alimentostica.com', N'Mensual', 22554477, 6);

-- Inserts en Beneficio
-- Para Grupo Zeta
INSERT INTO PlaniFy.Beneficio (idEmpresa, Nombre, TipoCalculo, Tipo)
VALUES
(1, N'Seguro Médico', N'Porcentaje', N'Salud'),
(1, N'Bono Productividad', N'Fijo', N'Financiero');

-- Para TechSolutions
INSERT INTO PlaniFy.Beneficio (idEmpresa, Nombre, TipoCalculo, Tipo)
VALUES
(2, N'Bono Transporte', N'Fijo', N'Logística'),
(2, N'Seguro Vida', N'Porcentaje', N'Salud');

-- Para Alimentos Tica
INSERT INTO PlaniFy.Beneficio (idEmpresa, Nombre, TipoCalculo, Tipo)
VALUES
(3, N'Bono Alimentación', N'Fijo', N'Alimentación'),
(3, N'Capacitación', N'Porcentaje', N'Profesional');

-- ===================================
-- CONSULTAS DE VERIFICACIÓN
-- ===================================
-- SELECT * FROM PlaniFy.Empresa;
-- SELECT * FROM PlaniFy.Beneficio;
-- SELECT * FROM PlaniFy.Direccion;
