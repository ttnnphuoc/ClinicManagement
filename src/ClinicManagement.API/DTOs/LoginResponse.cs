namespace ClinicManagement.API.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<ClinicInfo>? Clinics { get; set; }
}

public class ClinicInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}