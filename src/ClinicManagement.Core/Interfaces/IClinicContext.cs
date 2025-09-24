namespace ClinicManagement.Core.Interfaces;

public interface IClinicContext
{
    Guid? CurrentClinicId { get; }
    Guid? CurrentUserId { get; }
    string? CurrentUserRole { get; }
    void SetClinicId(Guid clinicId);
    void SetUserId(Guid userId);
    void SetUserRole(string role);
}