
using UsalClinic.Core.Entities;
using AspnetRun.Core.Repositories.Base;

namespace UsalClinic.Core.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetAllPatientAsync();
        Task<Patient?> GetByUserIdAsync(string userId);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetPatientsByDoctorUserIdAsync(string doctorUserId);
    }
}

