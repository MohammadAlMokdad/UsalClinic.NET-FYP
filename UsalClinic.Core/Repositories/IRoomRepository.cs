using UsalClinic.Core.Entities;
using AspnetRun.Core.Repositories.Base;

namespace UsalClinic.Core.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<IEnumerable<Room>> GetByDepartmentIdAsync(int departmentId);
    }
}
