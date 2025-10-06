namespace backend.Services.Interfaces
{
    public interface IAuthenticationService
    {
        bool IsValidToken(string? authHeader);
    }
}