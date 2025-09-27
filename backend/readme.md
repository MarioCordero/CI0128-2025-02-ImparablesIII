# Backend - Sistema de Gestión de Planillas y Beneficios

## 🚀 Descripción del Proyecto

Backend desarrollado en **ASP.NET Core** que sirve como motor principal para el sistema de gestión de planillas. Proporciona APIs RESTful seguras, lógica de negocio especializada y gestión completa de datos para el cálculo de deducciones y beneficios laborales.

---

## 📁 Estructura del Proyecto
backend/
├── 📋 Controllers/ # Endpoints de la API (RESTful)
├── ⚙️ Services/ # Lógica de negocio y cálculos
├── 🗃️ Repositories/ # Acceso a base de datos
├── 📊 Models/ # Entidades y DTOs
├── 🔧 Program.cs # Configuración y startup
├── ⚙️ appsettings.json # Configuración general
└── 📦 backend.csproj # Configuración del proyecto


### 🎯 Responsabilidades por Capa

| Capa | Responsabilidad | Ejemplos |
|------|----------------|----------|
| **Controllers** | Validación de requests, manejo de HTTP, seguridad | `DeduccionesController`, `BeneficiosController` |
| **Services** | Lógica de negocio, cálculos, validaciones | `CalculadoraDeduccionesService` |
| **Repositories** | Persistencia, consultas DB, transacciones | `EmpleadoRepository`, `PlanillaRepository` |
| **Models** | Estructuras de datos, DTOs, entidades | `Empleado`, `DeduccionDTO` |

---

## 🔐 Seguridad y Autenticación

### Autenticación Requerida
- **Todos los endpoints requieren token válido**
- Validación automática de JWT tokens
- Roles y permisos configurables

### Códigos de Respuesta HTTP
| Código | Significado | Escenario |
|--------|-------------|-----------|
| `200 OK` | Solicitud exitosa | Token válido + datos correctos |
| `400 Bad Request` | Parámetros inválidos | Datos faltantes o formato incorrecto |
| `403 Forbidden` | Acceso denegado | Token inválido o expirado |
| `500 Internal Server Error` | Error interno | Excepción no controlada |

---

## 📊 Endpoints Principales

### 🔍 Cálculo de Deducciones

```http
GET /api/deducciones/renta-empleado?salarioBruto=850000
GET /api/deducciones/salario?salarioBruto=850000
GET /api/aportes/empleado?salarioBruto=850000&cedulaEmpresa=123456789