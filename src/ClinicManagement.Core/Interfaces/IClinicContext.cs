namespace ClinicManagement.Core.Interfaces;

public interface IClinicContext
{
    Guid? CurrentClinicId { get; }
    void SetClinicId(Guid clinicId);
}