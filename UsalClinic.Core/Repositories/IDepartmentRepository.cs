using UsalClinic.Core.Entities;
using AspnetRun.Core.Repositories.Base;

namespace UsalClinic.Core.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department> GetByNameAsync(string name);  // Return a Department
        Task DeleteAsync(int id);
    }
}
