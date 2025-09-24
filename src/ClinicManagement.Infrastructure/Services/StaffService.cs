using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Enums;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IClinicRepository _clinicRepository;
    private readonly IAuthService _authService;

    public StaffService(
        IStaffRepository staffRepository,
        IClinicRepository clinicRepository,
        IAuthService authService)
    {
        _staffRepository = staffRepository;
        _clinicRepository = clinicRepository;
        _authService = authService;
    }

    public async Task<(bool Success, string? ErrorCode, Staff? Staff)> CreateStaffAsync(
        string fullName,
        string email,
        string password,
        string phoneNumber,
        string role,
        List<Guid> clinicIds,
        bool isActive)
    {
        var existingStaff = await _staffRepository.GetByEmailOrPhoneAsync(email);
        if (existingStaff != null)
        {
            return (false, "AUTH_EMAIL_EXISTS", null);
        }

        if (!Enum.TryParse<UserRole>(role, out var userRole))
        {
            return (false, "INVALID_INPUT", null);
        }

        var staff = new Staff
        {
            FullName = fullName,
            Email = email,
            PhoneNumber = phoneNumber,
            PasswordHash = _authService.HashPassword(password),
            Role = userRole,
            IsActive = isActive
        };

        await _staffRepository.AddAsync(staff);
        await _staffRepository.SaveChangesAsync();

        foreach (var clinicId in clinicIds)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId);
            if (clinic != null)
            {
                staff.StaffClinics.Add(new StaffClinic
                {
                    StaffId = staff.Id,
                    ClinicId = clinicId,
                    IsActive = true
                });
            }
        }

        await _staffRepository.SaveChangesAsync();

        var createdStaff = await _staffRepository.GetByIdWithClinicsAsync(staff.Id);
        return (true, null, createdStaff);
    }

    public async Task<(bool Success, string? ErrorCode, Staff? Staff)> UpdateStaffAsync(
        Guid id,
        string fullName,
        string email,
        string phoneNumber,
        string role,
        List<Guid> clinicIds,
        bool isActive)
    {
        var staff = await _staffRepository.GetByIdWithClinicsAsync(id);
        if (staff == null)
        {
            return (false, "NOT_FOUND", null);
        }

        if (!Enum.TryParse<UserRole>(role, out var userRole))
        {
            return (false, "INVALID_INPUT", null);
        }

        staff.FullName = fullName;
        staff.Email = email;
        staff.PhoneNumber = phoneNumber;
        staff.Role = userRole;
        staff.IsActive = isActive;

        staff.StaffClinics.Clear();
        foreach (var clinicId in clinicIds)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId);
            if (clinic != null)
            {
                staff.StaffClinics.Add(new StaffClinic
                {
                    StaffId = staff.Id,
                    ClinicId = clinicId,
                    IsActive = true
                });
            }
        }

        await _staffRepository.UpdateAsync(staff);
        await _staffRepository.SaveChangesAsync();

        var updatedStaff = await _staffRepository.GetByIdWithClinicsAsync(id);
        return (true, null, updatedStaff);
    }

    public async Task<(bool Success, string? ErrorCode)> ChangePasswordAsync(Guid id, string newPassword)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        if (staff == null)
        {
            return (false, "NOT_FOUND");
        }

        staff.PasswordHash = _authService.HashPassword(newPassword);
        await _staffRepository.UpdateAsync(staff);
        await _staffRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorCode)> DeleteStaffAsync(Guid id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        if (staff == null)
        {
            return (false, "NOT_FOUND");
        }

        await _staffRepository.SoftDeleteAsync(staff);
        await _staffRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Staff?> GetStaffByIdAsync(Guid id)
    {
        return await _staffRepository.GetByIdWithClinicsAsync(id);
    }

    public async Task<(IEnumerable<Staff> Items, int Total)> SearchStaffAsync(string? search, int page, int pageSize)
    {
        var items = await _staffRepository.SearchStaffAsync(search, page, pageSize);
        var total = await _staffRepository.GetTotalCountAsync(search);
        return (items, total);
    }
}