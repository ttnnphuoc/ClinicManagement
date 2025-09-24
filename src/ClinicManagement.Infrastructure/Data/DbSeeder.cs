using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Enums;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IAuthService authService)
    {
        // Seed default clinic if none exists
        if (!context.Clinics.Any())
        {
            var defaultClinic = new Clinic
            {
                Name = "Default Clinic",
                Address = "123 Main Street",
                PhoneNumber = "0123456789",
                Email = "clinic@example.com",
                IsActive = true
            };

            context.Clinics.Add(defaultClinic);
            await context.SaveChangesAsync();
        }

        // Seed super admin if none exists
        if (!context.Staff.Any())
        {
            var superAdmin = new Staff
            {
                FullName = "Super Admin",
                Email = "admin@clinic.com",
                PhoneNumber = "0000000000",
                PasswordHash = authService.HashPassword("Admin@123"),
                Role = UserRole.SuperAdmin,
                IsActive = true
            };

            context.Staff.Add(superAdmin);
            await context.SaveChangesAsync();
        }

        // Link admin to clinic if not already linked
        var admin = await context.Staff.FirstOrDefaultAsync(s => s.Email == "admin@clinic.com");
        var clinic = await context.Clinics.FirstOrDefaultAsync();
        
        if (admin != null && clinic != null && !context.StaffClinics.Any(sc => sc.StaffId == admin.Id))
        {
            var staffClinic = new StaffClinic
            {
                StaffId = admin.Id,
                ClinicId = clinic.Id,
                IsActive = true
            };

            context.StaffClinics.Add(staffClinic);
            await context.SaveChangesAsync();
        }
    }
}