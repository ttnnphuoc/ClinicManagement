using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class ClinicContext : IClinicContext
{
    private Guid? _currentClinicId;

    public Guid? CurrentClinicId => _currentClinicId;

    public void SetClinicId(Guid clinicId)
    {
        _currentClinicId = clinicId;
    }
}