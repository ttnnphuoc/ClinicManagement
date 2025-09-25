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

        // Seed subscription packages if none exist
        if (!context.SubscriptionPackages.Any())
        {
            var packages = new List<SubscriptionPackage>
            {
                new SubscriptionPackage
                {
                    Name = "Trial",
                    Description = "14-day free trial with basic features",
                    Price = 0,
                    DurationInDays = 14,
                    IsActive = true,
                    IsTrialPackage = true,
                    PackageLimits = new List<PackageLimit>
                    {
                        new PackageLimit { LimitType = "Clinics", LimitValue = 1, IsActive = true },
                        new PackageLimit { LimitType = "Patients", LimitValue = 50, IsActive = true },
                        new PackageLimit { LimitType = "Staff", LimitValue = 2, IsActive = true },
                        new PackageLimit { LimitType = "Appointments", LimitValue = 100, IsActive = true }
                    }
                },
                new SubscriptionPackage
                {
                    Name = "Basic",
                    Description = "Perfect for small clinics",
                    Price = 29.99m,
                    DurationInDays = 30,
                    IsActive = true,
                    IsTrialPackage = false,
                    PackageLimits = new List<PackageLimit>
                    {
                        new PackageLimit { LimitType = "Clinics", LimitValue = 1, IsActive = true },
                        new PackageLimit { LimitType = "Patients", LimitValue = 200, IsActive = true },
                        new PackageLimit { LimitType = "Staff", LimitValue = 5, IsActive = true },
                        new PackageLimit { LimitType = "Appointments", LimitValue = -1, IsActive = true }
                    }
                },
                new SubscriptionPackage
                {
                    Name = "Professional",
                    Description = "Advanced features for growing practices",
                    Price = 79.99m,
                    DurationInDays = 30,
                    IsActive = true,
                    IsTrialPackage = false,
                    PackageLimits = new List<PackageLimit>
                    {
                        new PackageLimit { LimitType = "Clinics", LimitValue = 3, IsActive = true },
                        new PackageLimit { LimitType = "Patients", LimitValue = 1000, IsActive = true },
                        new PackageLimit { LimitType = "Staff", LimitValue = 10, IsActive = true },
                        new PackageLimit { LimitType = "Appointments", LimitValue = -1, IsActive = true }
                    }
                },
                new SubscriptionPackage
                {
                    Name = "Premium",
                    Description = "Unlimited features for large organizations",
                    Price = 149.99m,
                    DurationInDays = 30,
                    IsActive = true,
                    IsTrialPackage = false,
                    PackageLimits = new List<PackageLimit>
                    {
                        new PackageLimit { LimitType = "Clinics", LimitValue = -1, IsActive = true },
                        new PackageLimit { LimitType = "Patients", LimitValue = -1, IsActive = true },
                        new PackageLimit { LimitType = "Staff", LimitValue = -1, IsActive = true },
                        new PackageLimit { LimitType = "Appointments", LimitValue = -1, IsActive = true }
                    }
                }
            };

            context.SubscriptionPackages.AddRange(packages);
            await context.SaveChangesAsync();
        }
    }
}