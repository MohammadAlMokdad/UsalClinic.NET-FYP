using AspnetRun.Core.Repositories.Base;
using UsalClinic.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UsalClinic.Core.Repositories
{
    public interface IShiftRepository : IRepository<Shift>
    {
        Task<IReadOnlyList<Shift>> GetAllAsync();
        Task<Shift?> GetByIdAsync(int id);
        Task<IReadOnlyList<Shift>> GetByStaffIdAsync(string staffId);
        Task<IReadOnlyList<Shift>> GetByRoleAsync(string role);
        Task<IReadOnlyList<Shift>> GetActiveNurseShiftsAsync(DayOfWeek day, TimeSpan currentTime);

    }
}
