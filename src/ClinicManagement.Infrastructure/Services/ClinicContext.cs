using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class ClinicContext : IClinicContext
{
    private Guid? _currentClinicId;
    private Guid? _currentUserId;
    private string? _currentUserRole;

    public Guid? CurrentClinicId => _currentClinicId;
    public Guid? CurrentUserId => _currentUserId;
    public string? CurrentUserRole => _currentUserRole;

    public void SetClinicId(Guid clinicId)
    {
        _currentClinicId = clinicId;
    }

    public void SetUserId(Guid userId)
    {
        _currentUserId = userId;
    }

    public void SetUserRole(string role)
    {
        _currentUserRole = role;
    }
}