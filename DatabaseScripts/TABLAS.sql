-- ===================================
-- (1) TABLA Dirección
-- ===================================
CREATE TABLE PlaniFy.Direccion (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    Provincia NVARCHAR(24),
    Canton NVARCHAR(60),
    Distrito NVARCHAR(60),
    DireccionParticular NVARCHAR(300),
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
-- (2) TABLA Persona
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(100) NOT NULL,
    Nombre NVARCHAR(40) NOT NULL,
    SegundoNombre NVARCHAR(40),
    Apellidos NVARCHAR(40) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(40),
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);

-- ===================================
-- (3) TABLA Usuario
-- ===================================
CREATE TABLE PlaniFy.Usuario (
    idPersona INT PRIMARY KEY NOT NULL,
    TipoUsuario NVARCHAR(40) NOT NULL,
    Contrasena NVARCHAR(32) NOT NULL,
    VerificationTokenHash NVARCHAR(128),
    VerificationTokenExpires DATETIME,
    IsVerified BIT DEFAULT 0,
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

-- ===================================
-- (4) TABLA Empresa
-- ===================================
CREATE TABLE PlaniFy.Empresa (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    Nombre NVARCHAR(40) NOT NULL,
    CedulaJuridica INT NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PeriodoPago NVARCHAR(40) NOT NULL,
    Telefono INT,
    idDireccion INT NOT NULL,
    idEmpleador INT NOT NULL,
    MaximoBeneficios INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(Id),
    FOREIGN KEY (idEmpleador) REFERENCES PlaniFy.Persona(Id),
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (N'Mensual', N'Quincenal')
    )
);

-- ===================================
-- (5) TABLA Empleado
-- ===================================
CREATE TABLE PlaniFy.Empleado (
    idPersona INT PRIMARY KEY NOT NULL,
    Departamento NVARCHAR(40) NOT NULL,
    TipoContrato NVARCHAR(50) NOT NULL,
    TipoSalario NVARCHAR(20),
    Puesto NVARCHAR(40) NOT NULL,
    FechaContratacion DATE NOT NULL,
    Salario INT NOT NULL,
    iban NVARCHAR(60) NOT NULL,
    Contrasena NVARCHAR(32),
    idEmpresa INT,
    FechaBaja DATETIME,
    MotivoBaja NVARCHAR(100),
    UsuarioBajaId INT,
    Estado NVARCHAR(20) DEFAULT 'Activo',
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
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
-- (6) TABLA EmpleadoEmpresa
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);

-- ===================================
-- (7) TABLA Planilla
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
-- (8) TABLA Beneficio
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    TipoCalculo NVARCHAR(40) NOT NULL,
    Tipo NVARCHAR(40) NOT NULL,
    Valor INT,
    Porcentaje INT,
    Descripcion VARCHAR(200),
    IsDeleted BIT DEFAULT 0,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE
);

-- ===================================
-- (9) TABLA BeneficioEmpleado
-- ===================================
CREATE TABLE PlaniFy.BeneficioEmpleado (
    idEmpleado INT NOT NULL,
    NombreBeneficio NVARCHAR(100) NOT NULL,
    idEmpresa INT NOT NULL,
    TipoBeneficio NVARCHAR(40) NOT NULL,
    NumeroDependientes INT,
    TipoPension NVARCHAR(2),
    PRIMARY KEY (idEmpleado, NombreBeneficio, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
    FOREIGN KEY (idEmpresa, NombreBeneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre) ON DELETE CASCADE,
    CONSTRAINT CHK_BeneficioEmpleado_NumeroDependientes CHECK (NumeroDependientes IS NULL OR NumeroDependientes >= 0),
    CONSTRAINT CHK_BeneficioEmpleado_TipoPension CHECK (TipoPension IS NULL OR (TipoPension='C' OR TipoPension='B' OR TipoPension='A'))
);

-- ===================================
-- (10) TABLA HorasTrabajadas
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL,
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(300) NOT NULL,
    Fecha DATE NOT NULL,
    Estado VARCHAR(9) NOT NULL DEFAULT 'Pendiente',
    idAprobador INT NOT NULL,
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id),
    CONSTRAINT CK_HorasTrabajadas_Estado CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado'))
);

-- ===================================
-- (11) TABLA DetallePlanilla
-- ===================================
CREATE TABLE PlaniFy.DetallePlanilla (
    idEmpleado INT,
    idPlanilla INT,
    salarioBruto INT,
    totalBeneficios INT,
    DeduccionesEmpleado INT,
    salarioNeto INT,
    DeduccionesEmpresa INT,
    Puesto NVARCHAR(40),
    PRIMARY KEY (idEmpleado, idPlanilla),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id)
);

-- ===================================
-- (12) TABLA Deducciones
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(40) NOT NULL,
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(40),
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    FOREIGN KEY (idEmpresa, Beneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE SET NULL
);

-- ===================================
-- (13) TABLA EmployeeDeductions
-- ===================================
CREATE TABLE PlaniFy.EmployeeDeductions (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Code NVARCHAR(64) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    IsActive BIT DEFAULT 1 NOT NULL
);

-- ===================================
-- (14) TABLA EmployerDeductions
-- ===================================
CREATE TABLE PlaniFy.EmployerDeductions (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Code NVARCHAR(64) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    Rate DECIMAL(8,5) NOT NULL,
    MinAmount DECIMAL(18,2),
    MaxAmount DECIMAL(18,2),
    IsActive BIT DEFAULT 1 NOT NULL
);

-- ===================================
-- (15) TABLA ResumenPlanilla
-- ===================================
CREATE TABLE PlaniFy.ResumenPlanilla (
    idPlanilla INT NOT NULL,
    idEmpresa INT NOT NULL,
    TotalSalarioBruto INT NOT NULL,
    TotalDeduccionesEmpleado INT NOT NULL,
    TotalDeduccionesEmpresa INT NOT NULL,
    TotalBeneficios INT NOT NULL,
    TotalSalarioNeto INT NOT NULL,
    PRIMARY KEY (idPlanilla),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);