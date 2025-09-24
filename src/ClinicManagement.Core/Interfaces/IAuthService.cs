namespace ClinicManagement.Core.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Token, string FullName, string Email, string Role)> LoginAsync(string emailOrPhone, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}