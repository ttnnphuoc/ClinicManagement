namespace ClinicManagement.API.DTOs;

public class SelectClinicRequest
{
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
}