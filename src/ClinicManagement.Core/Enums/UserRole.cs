namespace ClinicManagement.Core.Enums;

public enum UserRole
{
    SuperAdmin = 0,    // Changed to 0 to match database
    ClinicManager = 1,
    Doctor = 2,
    Nurse = 3,
    Receptionist = 4,
    Accountant = 5,
    Pharmacist = 6
}