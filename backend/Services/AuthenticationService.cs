using backend.Services.Interfaces;

namespace backend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // This class only validates validation and authentication of the token format. (SRP)
        private const string HARDCODED_TOKEN = "ImparablesToken2025";
        
        public bool IsValidToken(string? authHeader)
        {
            // Validate presence and format
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return false;

            // Extract token
            string token = authHeader.Substring("Bearer ".Length).Trim();

            // Validate that it matches the hardcoded token
            return string.Equals(token, HARDCODED_TOKEN, StringComparison.Ordinal);
        }
    }
}