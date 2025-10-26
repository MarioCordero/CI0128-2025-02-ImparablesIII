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

-- ===================================
-- (2) TABLA Persona (Chris)
-- ===================================
CREATE TABLE PlaniFy.Persona (
    Id INT IDENTITY PRIMARY KEY NOT NULL,
    Correo NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    SegundoNombre NVARCHAR(20),
    Apellidos NVARCHAR(20) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Cedula CHAR(9) NOT NULL,
    Rol NVARCHAR(20) NOT NULL,
    Telefono INT,
    idDireccion INT,
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Persona_Cedula CHECK (Cedula NOT LIKE '%[^0-9]%')
);

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
    FOREIGN KEY (idDireccion) REFERENCES PlaniFy.Direccion(id),
    CONSTRAINT CK_Periodo_Pago CHECK (
        PeriodoPago IN (
            N'Mensual',
            N'Quincenal'
        )
    )
);

-- ===================================
-- (5) TABLA Empleado (Chino)
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
    idEmpresa INT NOT NULL,
    FOREIGN KEY (idPersona) REFERENCES PlaniFy.Persona(Id) ON DELETE CASCADE, -- Si se borra la persona, se borra el empleado
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE, -- Si se borra la empresa, se borran los empleados
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
-- (6) TABLA EmpleadoEmpresa (Chino)
-- ===================================
CREATE TABLE PlaniFy.EmpleadoEmpresa (
    idEmpleado INT NOT NULL,
    idEmpresa INT NOT NULL,
    PRIMARY KEY (idEmpleado, idEmpresa),
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona) ON DELETE CASCADE, -- Si se borra el empleado, se borra la relacion
    -- VIEJO: FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE CASCADE -- Si se borra la empresa, se borran las relaciones
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) ON DELETE NO ACTION -- Cambiar a NO ACTION para evitar ciclo de los ON CASCADE
);

-- ===================================
-- (7) TABLA Planilla (Mario)
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
-- (8) TABLA Beneficio (Chino)
-- ===================================
CREATE TABLE PlaniFy.Beneficio (
    idEmpresa INT NOT NULL,
    Nombre NVARCHAR(20) NOT NULL,
    TipoCalculo NVARCHAR(20) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    Valor INT,
    Porcentaje INT,
    PRIMARY KEY (idEmpresa, Nombre),
    FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id)
        ON DELETE CASCADE
);

-- ===================================
-- (9) TABLA BeneficioEmpleado (Mario)
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
-- (10) TABLA HorasTrabajadas (Mario)
-- ===================================
CREATE TABLE PlaniFy.HorasTrabajadas (
    -- id INT IDENTITY PRIMARY KEY NOT NULL,
    id INT IDENTITY NOT NULL,
    idEmpleado INT NOT NULL,
    Cantidad INT NOT NULL,
    Detalle NVARCHAR(150) NOT NULL,
    Estado BIT DEFAULT 0,
    idAprobador INT NOT NULL,
    FOREIGN KEY (idEmpleado) REFERENCES PlaniFy.Empleado(idPersona),
    -- FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Empleado(idPersona) -- SEGUN EL MODELO RELACIONAL EL ID VIENE REFERENCIADO DE PERSONA, NO DE EMPLEADO
    -- CAMBIOS
    PRIMARY KEY (id, idEmpleado),
    FOREIGN KEY (idAprobador) REFERENCES PlaniFy.Persona(Id)
);

-- ===================================
-- (11) TABLA DetallePlanilla (Diego)
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
-- (12) TABLA Deducciones (Diego)
-- ===================================
CREATE TABLE PlaniFy.Deducciones (
    idPlanilla INT,
    Nombre NVARCHAR(20),
    Valor INT,
    idEmpresa INT,
    Beneficio NVARCHAR(20),
    PRIMARY KEY (idPlanilla, Nombre),
    FOREIGN KEY (idPlanilla) REFERENCES PlaniFy.Planilla(id),
    -- FOREIGN KEY (idEmpresa) REFERENCES PlaniFy.Empresa(Id) -- SEGUN EL MODELO RELACIONAL EL ID VIENE REFERENCIADO DE BENEFICIO, NO DE EMPRESA
    -- NUEVO
    FOREIGN KEY (idEmpresa, Beneficio) REFERENCES PlaniFy.Beneficio(idEmpresa, Nombre)
        ON DELETE SET NULL
);