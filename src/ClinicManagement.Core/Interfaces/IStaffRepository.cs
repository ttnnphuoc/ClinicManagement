using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByEmailOrPhoneAsync(string emailOrPhone);
    Task<Staff?> GetByEmailOrPhoneWithClinicsAsync(string emailOrPhone);
    Task<bool> HasAccessToClinicAsync(Guid staffId, Guid clinicId);
}