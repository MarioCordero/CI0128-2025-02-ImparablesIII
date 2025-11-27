-- ===================================
-- (1) TABLA Dirección CORREGIDA
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

-- ===================================
-- (2) TABLA Persona CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(100) NOT NULL, -- CORREGIDO: 50 → 100
    Nombre NVARCHAR(40) NOT NULL,  -- CORREGIDO: 20 → 40
    SegundoNombre NVARCHAR(40),    -- CORREGIDO: 20 → 40
    Apellidos NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(40),              -- CORREGIDO: 20 → 40, NULLABLE
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);

-- ===================================
-- (3) TABLA Usuario CORREGIDA
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

-- ===================================
-- (4) TABLA Empresa CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.Empresa (
    Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    -- Datos básicos de la empresa
    Nombre NVARCHAR(40) NOT NULL,
    CedulaJuridica INT NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PeriodoPago NVARCHAR(40) NOT NULL,  -- ('Mensual', 'Quincenal')
    Telefono INT,
    -- Relaciones
    idDireccion INT NOT NULL,           -- FK a Direccion
    idEmpleador INT NOT NULL,           -- FK a Persona (rol: Empleador)
    -- Configuración interna
    MaximoBeneficios INT,
    -- Foreign Keys
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(Id),
    FOREIGN KEY (idEmpleador) REFERENCES PlaniFy.Persona(Id),
    -- Restricciones
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (N'Mensual', N'Quincenal')
    )
);


-- ===================================
-- (5) TABLA Empleado CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.Empleado (
    idPersona INT PRIMARY KEY NOT NULL,
    Departamento NVARCHAR(40) NOT NULL, -- CORREGIDO: 20 → 40
    TipoContrato NVARCHAR(50) NOT NULL, -- CORREGIDO: 25 → 50
    TipoSalario NVARCHAR(20),           -- CORREGIDO: 10 → 20
    Puesto NVARCHAR(40) NOT NULL,       -- CORREGIDO: 20 → 40
    FechaContratacion DATE NOT NULL,
    Salario INT NOT NULL,
    iban NVARCHAR(60) NOT NULL,         -- CORREGIDO: 30 → 60
    Contrasena NVARCHAR(32),            -- CORREGIDO: 16 → 32
    idEmpresa INT,                      -- CORREGIDO: NOT NULL → NULLABLE
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
-- (6) TABLA EmpleadoEmpresa CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE,
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
);

-- ===================================
-- (7) TABLA Planilla CORREGIDA
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
-- (8) TABLA Beneficio CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,      -- CORREGIDO: 20 → 100
    TipoCalculo NVARCHAR(40) NOT NULL,  -- CORREGIDO: 20 → 40
    Tipo NVARCHAR(40) NOT NULL,         -- CORREGIDO: 20 → 40
    Valor INT,
    Porcentaje INT,
    Descripcion VARCHAR(200),
    IsDeleted BIT DEFAULT 0,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE
);

-- ===================================
-- (9) TABLA BeneficioEmpleado CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.BeneficioEmpleado (
    idEmpleado INT NOT NULL,
    NombreBeneficio NVARCHAR(100) NOT NULL, -- CORREGIDO: 20 → 100
    idEmpresa INT NOT NULL,
    TipoBeneficio NVARCHAR(40) NOT NULL,    -- CORREGIDO: 20 → 40
    NumeroDependientes INT,                 -- AÑADIDO: Falta en tu script
    TipoPension NVARCHAR(2),                -- AÑADIDO: Falta en tu script
    PRIMARY KEY (idEmpleado, NombreBeneficio, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id),
    FOREIGN KEY (idEmpresa, NombreBeneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre) ON DELETE CASCADE,
    CONSTRAINT CHK_BeneficioEmpleado_NumeroDependientes CHECK (NumeroDependientes IS NULL OR NumeroDependientes >= 0),
    CONSTRAINT CHK_BeneficioEmpleado_TipoPension CHECK (TipoPension IS NULL OR (TipoPension='C' OR TipoPension='B' OR TipoPension='A'))
);

-- ===================================
-- (10) TABLA HorasTrabajadas CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    id INT IDENTITY PRIMARY KEY NOT NULL, -- CORREGIDO: PK simple (no compuesta)
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(300) NOT NULL,       -- CORREGIDO: 150 → 300
    Fecha DATE NOT NULL,
    Estado VARCHAR(9) NOT NULL DEFAULT 'Pendiente', -- CORREGIDO: BIT → VARCHAR(9) con default 'Pendiente'
    idAprobador INT NOT NULL,

    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id),
    CONSTRAINT CK_HorasTrabajadas_Estado CHECK (Estado IN ('Pendiente', 'Aprobado', 'Rechazado'))
);

-- ===================================
-- (11) TABLA DetallePlanilla CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.DetallePlanilla (
    idEmpleado INT,
    idPlanilla INT,
    salarioBruto INT,
    totalBeneficios INT,
    DeduccionesEmpleado INT,              -- AÑADIDO: Falta en tu script
    salarioNeto INT,                      -- AÑADIDO: Falta en tu script
    DeduccionesEmpresa INT,               -- AÑADIDO: Falta en tu script
    PRIMARY KEY (idEmpleado, idPlanilla),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id)
);

-- ===================================
-- (12) TABLA Deducciones CORREGIDA
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(40) NOT NULL,         -- CORREGIDO: 20 → 40
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(40),               -- CORREGIDO: 20 → 40
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    FOREIGN KEY (idEmpresa, Beneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE SET NULL
);

-- ===================================
-- (13) TABLA EmployeeDeductions (NUEVA)
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
-- (14) TABLA EmployerDeductions (NUEVA)
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
-- (15) TABLA ResumenPlanilla (NUEVA)
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