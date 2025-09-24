using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IStaffService
{
    Task<(bool Success, string? ErrorCode, Staff? Staff)> CreateStaffAsync(
        string fullName, 
        string email, 
        string password, 
        string phoneNumber, 
        string role, 
        List<Guid> clinicIds, 
        bool isActive);

    Task<(bool Success, string? ErrorCode, Staff? Staff)> UpdateStaffAsync(
        Guid id, 
        string fullName, 
        string email, 
        string phoneNumber, 
        string role, 
        List<Guid> clinicIds, 
        bool isActive);

    Task<(bool Success, string? ErrorCode)> ChangePasswordAsync(Guid id, string newPassword);

    Task<(bool Success, string? ErrorCode)> DeleteStaffAsync(Guid id);

    Task<Staff?> GetStaffByIdAsync(Guid id);

    Task<(IEnumerable<Staff> Items, int Total)> SearchStaffAsync(string? search, int page, int pageSize);
}