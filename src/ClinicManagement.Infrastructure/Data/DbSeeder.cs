using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IAuthService authService)
    {
        if (!context.Staff.Any())
        {
            var superAdmin = new Staff
            {
                FullName = "Super Admin",
                Email = "admin@clinic.com",
                PhoneNumber = "0000000000",
                PasswordHash = authService.HashPassword("Admin@123"),
                Role = "SuperAdmin",
                IsActive = true
            };

            context.Staff.Add(superAdmin);
            await context.SaveChangesAsync();
        }
    }
}