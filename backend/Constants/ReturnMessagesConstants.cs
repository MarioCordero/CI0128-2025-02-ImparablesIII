namespace backend.Constants
{
    public static class ReturnMessagesConstants
    {
        // General Error Messages
        public static class General
        {
            public const string InternalServerError = "Error interno del servidor";
            public const string InternalServerErrorWithDetail = "Error interno del servidor: {0}";
            public const string InvalidRequest = "Solicitud inválida";
            public const string NotFound = "Recurso no encontrado";
            public const string EmailAlreadyExists = "El correo electrónico se encuentra registrado en el sistema.";
        }

        // Validation Messages
        public static class Validation
        {
            public const string EmployeeIdMustBeGreaterThanZero = "El ID del empleado debe ser mayor a 0";
            public const string AuthenticatedEmployeeIdRequired = "El ID del empleado autenticado es requerido";
            public const string CompanyIdInvalid = "Id de empresa inválido.";
            public const string ResponsibleEmployeeIdInvalid = "Id de empleado responsable inválido.";
            public const string HoursMustBeGreaterThanZero = "Horas deben ser mayor a cero.";
            public const string EmailAndPasswordRequired = "Correo y contraseña son requeridos";
            public const string InvalidOrMissingEmployerId = "Id de empresa inválido o faltante";
        }

        // Employee Messages
        public static class Employee
        {
            public const string EmployeeNotFound = "Empleado no encontrado";
            public const string EmployeeNotFoundOrNoCompany = "Empleado no encontrado o no tiene empresa asignada";
            public const string EmployeeRegisteredSuccessfully = "Empleado registrado exitosamente. Se ha enviado un correo para configurar la contraseña.";
            public const string CedulaAlreadyRegistered = "La cédula ya está registrada en el sistema.";
        }

        // Benefit Messages
        public static class Benefit
        {
            public const string BenefitNotFound = "Beneficio no encontrado";
            public const string ErrorRetrievingBenefits = "Error al obtener los beneficios";
            public const string ErrorRetrievingBenefit = "Error al obtener el beneficio";
            public const string ErrorCreatingBenefit = "Error al crear el beneficio";
            public const string ErrorCheckingBenefitExistence = "Error al verificar la existencia del beneficio";
            public const string ErrorUpdatingBenefit = "Error al actualizar el beneficio";
            public const string ErrorSelectingBenefit = "Error al seleccionar el beneficio";
            public const string ErrorValidatingSelection = "Error al validar la selección";
        }

        // Project Messages
        public static class Project
        {
            public const string ProjectNotFound = "Empresa no encontrada";
            public const string ErrorRetrievingProjects = "Error al obtener las empresas";
            public const string ErrorRetrievingProject = "Error al obtener la empresa";
            public const string LegalIdAlreadyExists = "La cédula jurídica ya existe";
            public const string ErrorCreatingProject = "Error al crear la empresa";
            public const string ErrorDeletingProject = "Error al eliminar la empresa";
            public const string ErrorCountingEmployees = "Error al contar los empleados";
            public const string ProjectActivatedSuccessfully = "Empresa activada exitosamente";
            public const string ErrorActivatingProject = "Error al activar la empresa";
            public const string ProjectDeactivatedSuccessfully = "Empresa desactivada exitosamente";
            public const string ErrorDeactivatingProject = "Error al desactivar la empresa";
        }

        // Payroll Messages
        public static class Payroll
        {
            public const string PayrollGeneratedSuccessfully = "Planilla generada exitosamente con beneficios y deducciones.";
            public const string ErrorGeneratingPayroll = "Error al generar la planilla.";
            public const string ReportNotFound = "Reporte no encontrado";
        }

        // User Messages
        public static class User
        {
            public const string UserNotFound = "Usuario no encontrado";
            public const string UserRegisteredSuccessfully = "Usuario registrado exitosamente y se ha enviado un correo de bienvenida.";
            public const string UserRegisteredButEmailFailed = "Usuario registrado exitosamente pero no se pudo enviar el correo de bienvenida.";
            public const string RegistrationFailed = "Error al registrar el usuario.";
            public const string PasswordResetEmailSent = "Correo de restablecimiento de contraseña enviado exitosamente.";
            public const string PasswordResetEmailFailed = "Error al enviar el correo de restablecimiento de contraseña.";
            public const string PasswordResetFailed = "Error al restablecer la contraseña.";
        }

        // Login Messages
        public static class Login
        {
            public const string InvalidCredentials = "Credenciales inválidas";
        }

        // Dashboard Messages
        public static class Dashboard
        {
            public const string ErrorRetrievingDashboardData = "Error al obtener los datos del dashboard";
        }

        // Profile Messages
        public static class Profile
        {
            public const string ProfileUpdatedSuccessfully = "Perfil actualizado exitosamente";
        }

        // Password Setup Messages
        public static class PasswordSetup
        {
            public const string PasswordSetupSuccess = "Contraseña configurada exitosamente";
            public const string PasswordSetupFailed = "Error al configurar la contraseña";
        }

        // Email Messages
        public static class Email
        {
            public const string EmailServiceRunning = "Servicio de correo electrónico funcionando correctamente";
        }
    }
}

