using AspnetRun.Core.Repositories.Base;
using UsalClinic.Core.Entities;

namespace UsalClinic.Core.Repositories
{
    public interface INurseRepository : IRepository<Nurse>
    {
        Task<IReadOnlyList<Nurse>> GetAllAsync();
        Task<Nurse?> GetByIdAsync(Guid id);
        Task<Nurse?> GetByUserIdAsync(string userId);
    }
}
