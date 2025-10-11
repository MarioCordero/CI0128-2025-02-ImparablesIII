namespace ExternalAPI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        bool IsValidToken(string? authHeader);
    }
}