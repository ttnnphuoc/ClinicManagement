using ClinicManagement.Core.Entities;

namespace ClinicManagement.Core.Interfaces;

public interface IMedicineRepository : IRepository<Medicine>
{
    Task<IEnumerable<Medicine>> GetActiveMedicinesAsync();
    Task<Medicine?> GetMedicineWithInventoryAsync(Guid id);
    Task<IEnumerable<Medicine>> SearchMedicinesAsync(string searchTerm);
}