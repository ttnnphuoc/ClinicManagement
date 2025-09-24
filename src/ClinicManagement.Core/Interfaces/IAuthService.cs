namespace ClinicManagement.Core.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Token, string FullName, string Email, string Role, List<Guid> ClinicIds)> LoginAsync(string emailOrPhone, string password);
    Task<(bool Success, string Token, string FullName, string Email, string Role, Guid ClinicId)> LoginWithClinicAsync(string emailOrPhone, string password, Guid clinicId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}