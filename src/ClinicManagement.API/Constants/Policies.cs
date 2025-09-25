namespace ClinicManagement.API.Constants;

public static class Policies
{
    // Role-based policies
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string ClinicManagerOnly = "ClinicManagerOnly";
    public const string DoctorOnly = "DoctorOnly";
    public const string NurseOnly = "NurseOnly";
    public const string ReceptionistOnly = "ReceptionistOnly";
    public const string AccountantOnly = "AccountantOnly";
    public const string PharmacistOnly = "PharmacistOnly";

    // Combined policies
    public const string ManageClinic = "ManageClinic"; // SuperAdmin + ClinicManager
    public const string ManagePatients = "ManagePatients"; // SuperAdmin + ClinicManager + Doctor + Nurse + Receptionist
    public const string ManageAppointments = "ManageAppointments"; // SuperAdmin + ClinicManager + Doctor + Nurse + Receptionist
    public const string ViewPatientRecords = "ViewPatientRecords"; // SuperAdmin + ClinicManager + Doctor + Nurse
    public const string ManageServices = "ManageServices"; // SuperAdmin + ClinicManager
    public const string ManageFinance = "ManageFinance"; // SuperAdmin + ClinicManager + Accountant
    public const string ManageInventory = "ManageInventory"; // SuperAdmin + ClinicManager + Pharmacist
    public const string ManageStaff = "ManageStaff"; // SuperAdmin + ClinicManager
    public const string ManageBills = "ManageBills"; // SuperAdmin + ClinicManager + Accountant + Receptionist
}