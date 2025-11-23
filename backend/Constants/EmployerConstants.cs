namespace backend.Constants
{
    public static class EmployerConstants
    {
        public static class Employer
        {
            public const string EmployerRegistrationFailed = "No se pudo registrar el empleador.";
            public const string EmployerRegisteredSuccessfully = "Empleador registrado correctamente.";
            public const string VerificationResentFailed = "No se pudo reenviar verificación.";
            public const string VerificationResentSuccess = "Correo de verificación reenviado.";
            public const string VerificationFailed = "Verificación fallida.";
            public const string VerificationSuccess = "Cuenta verificada correctamente.";
            public const string CodeInvalidOrExpired = "Código inválido o expirado.";
            public const string CodeVerified = "Código verificado.";
        }

        public static class Validation
        {
            public const string ValidationErrors = "Errores de validación.";
            public const string InvalidData = "Dato inválido.";
            public const string EmailRequired = "Email requerido.";
            public const string PersonaIdPasswordRequired = "PersonaId y password son requeridos.";
            public const string EmailAndCodeRequired = "Email y código son requeridos.";
        }

        public static class General
        {
            public const string InternalServerError = "Error interno del servidor.";
        }
    }
}