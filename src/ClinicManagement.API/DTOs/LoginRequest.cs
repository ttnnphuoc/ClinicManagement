namespace ClinicManagement.API.DTOs;

public class LoginRequest
{
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}