using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<(bool Success, string? ErrorCode, Patient? Patient)> CreatePatientAsync(
        Guid clinicId,
        string fullName,
        string phoneNumber,
        string? email,
        DateTime? dateOfBirth,
        string? address,
        string? gender,
        string? allergies,
        string? chronicConditions,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? bloodType,
        string? idNumber,
        string? insuranceNumber,
        string? insuranceProvider,
        string? occupation,
        string? referralSource,
        bool receivePromotions,
        string? notes)
    {
        var clinicPatientCount = await _patientRepository.GetClinicPatientCountAsync(clinicId);
        var patientCount = clinicPatientCount + 1;

        var patient = new Patient
        {
            ClinicId = clinicId,
            PatientCode = $"PT{patientCount:D5}",
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            DateOfBirth = dateOfBirth,
            Address = address,
            Gender = gender,
            Allergies = allergies,
            ChronicConditions = chronicConditions,
            EmergencyContactName = emergencyContactName,
            EmergencyContactPhone = emergencyContactPhone,
            BloodType = bloodType,
            IdNumber = idNumber,
            InsuranceNumber = insuranceNumber,
            InsuranceProvider = insuranceProvider,
            Occupation = occupation,
            ReferralSource = referralSource,
            FirstVisitDate = DateTime.UtcNow,
            ReceivePromotions = receivePromotions,
            Notes = notes
        };

        await _patientRepository.AddAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return (true, null, patient);
    }

    public async Task<(bool Success, string? ErrorCode, Patient? Patient)> UpdatePatientAsync(
        Guid id,
        string fullName,
        string phoneNumber,
        string? email,
        DateTime? dateOfBirth,
        string? address,
        string? gender,
        string? allergies,
        string? chronicConditions,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? bloodType,
        string? idNumber,
        string? insuranceNumber,
        string? insuranceProvider,
        string? occupation,
        string? referralSource,
        bool receivePromotions,
        string? notes)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return (false, "NOT_FOUND", null);
        }

        patient.FullName = fullName;
        patient.PhoneNumber = phoneNumber;
        patient.Email = email;
        patient.DateOfBirth = dateOfBirth;
        patient.Address = address;
        patient.Gender = gender;
        patient.Allergies = allergies;
        patient.ChronicConditions = chronicConditions;
        patient.EmergencyContactName = emergencyContactName;
        patient.EmergencyContactPhone = emergencyContactPhone;
        patient.BloodType = bloodType;
        patient.IdNumber = idNumber;
        patient.InsuranceNumber = insuranceNumber;
        patient.InsuranceProvider = insuranceProvider;
        patient.Occupation = occupation;
        patient.ReferralSource = referralSource;
        patient.ReceivePromotions = receivePromotions;
        patient.Notes = notes;

        await _patientRepository.UpdateAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return (true, null, patient);
    }

    public async Task<(bool Success, string? ErrorCode)> DeletePatientAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return (false, "NOT_FOUND");
        }

        await _patientRepository.SoftDeleteAsync(patient);
        await _patientRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<Patient?> GetPatientByIdAsync(Guid id)
    {
        return await _patientRepository.GetByIdAsync(id);
    }

    public async Task<(IEnumerable<Patient> Items, int Total)> SearchPatientsAsync(string? search, int page, int pageSize)
    {
        var items = await _patientRepository.SearchPatientsAsync(search, page, pageSize);
        var total = await _patientRepository.GetTotalCountAsync(search);
        return (items, total);
    }
}