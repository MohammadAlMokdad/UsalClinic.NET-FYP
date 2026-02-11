using UsalClinic.Core.Entities;

public interface IPrescriptionRepository
{
    Task<IEnumerable<Prescription>> GetAllAsync();
    Task<Prescription> GetByIdAsync(int id);
    Task AddAsync(Prescription prescription);
    Task UpdateAsync(Prescription prescription);
    Task DeleteAsync(int id);
    Task<IEnumerable<Prescription>> GetPrescriptionsByMedicalRecordIdAsync(int medicalId);
}
