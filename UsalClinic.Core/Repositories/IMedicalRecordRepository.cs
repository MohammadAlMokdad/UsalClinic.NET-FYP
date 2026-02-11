using UsalClinic.Core.Entities;
using AspnetRun.Core.Repositories.Base;

namespace UsalClinic.Core.Repositories
{
    public interface IMedicalRecordRepository : IRepository<MedicalRecord>
    {
        Task<IReadOnlyList<MedicalRecord>> GetAllMedicalRecordsAsync();
        Task<MedicalRecord?> GetByDoctorAndPatientAsync(Guid doctorId, int patientId);
        Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId);
        Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id);
        Task<MedicalRecord?> GetMedicalRecordByPatientIdAsync(int patientId);
    }
}
