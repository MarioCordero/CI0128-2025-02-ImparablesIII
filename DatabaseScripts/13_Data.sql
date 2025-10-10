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