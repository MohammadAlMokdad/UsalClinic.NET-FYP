using AspnetRun.Core.Repositories.Base;
using UsalClinic.Core.Entities;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<Doctor?> GetDoctorByIdAsync(Guid id);
    Task<IReadOnlyList<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetByUserIdAsync(string userId);
}
