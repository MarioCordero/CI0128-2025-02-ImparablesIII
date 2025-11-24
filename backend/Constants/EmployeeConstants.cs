namespace backend.Constants
{
    public static class EmployeeConstants
    {
        // Existing constants
        public const string StatusActive = "Activo";
        public const string StatusInactive = "Inactivo";
        public const string SalaryTypeMonthly = "Mensual";
        public const string PersonTypeEmployee = "Empleado";
        public const string DefaultDeletionReason = "Sin motivo especificado";

        // New constants for registration and messages
        public static class Employee
        {
            public const string EmployeeRegisteredSuccessfully = "Empleado registrado exitosamente";
            public const string EmployeeRegistrationFailed = "Error al registrar el empleado";
            public const string CedulaAlreadyRegistered = "La cédula ya está registrada";
            public const string EmployeeNotFound = "Empleado no encontrado";
            public const string EmployeeDeletedSuccessfully = "Empleado eliminado exitosamente";
            public const string EmployeeDeletionFailed = "Error al eliminar el empleado";
        }

        public static class Validation
        {
            public const string InvalidData = "Datos inválidos";
            public const string ValidationErrors = "Errores de validación";
            public const string CedulaRequired = "La cédula es requerida";
            public const string EmailRequired = "El email es requerido";
            public const string NameRequired = "El nombre es requerido";
        }
    }
}