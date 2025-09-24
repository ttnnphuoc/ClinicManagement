using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IStaffRepository : IRepository<Staff>
{
    Task<Staff?> GetByEmailOrPhoneAsync(string emailOrPhone);
    Task<Staff?> GetByEmailOrPhoneWithClinicsAsync(string emailOrPhone);
    Task<bool> HasAccessToClinicAsync(Guid staffId, Guid clinicId);
    Task<Staff?> GetByIdWithClinicsAsync(Guid id);
    Task<int> GetTotalCountAsync(string? search = null);
    Task<IEnumerable<Staff>> SearchStaffAsync(string? search, int page, int pageSize);
}