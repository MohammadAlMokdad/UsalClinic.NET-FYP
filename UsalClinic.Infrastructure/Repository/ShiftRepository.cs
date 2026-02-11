using Microsoft.EntityFrameworkCore;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure.Repository.Base;
using UsalClinic.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UsalClinic.Infrastructure.Repository
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Shift>> GetAllAsync()
        {
            return await _dbContext.Shifts
                .Include(s => s.Staff)
                .ToListAsync();
        }

        public async Task<Shift?> GetByIdAsync(int id)
        {
            return await _dbContext.Shifts
                .Include(s => s.Staff)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IReadOnlyList<Shift>> GetByStaffIdAsync(string staffId)
        {
            return await _dbContext.Shifts
                .Include(s => s.Staff)
                .Where(s => s.StaffId == staffId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Shift>> GetByRoleAsync(string role)
        {
            return await _dbContext.Shifts
                .Include(s => s.Staff)
                .Where(s => s.Role == role)
                .ToListAsync();
        }
        public async Task<IReadOnlyList<Shift>> GetActiveNurseShiftsAsync(DayOfWeek day, TimeSpan currentTime)
        {
            return await _dbContext.Shifts
                .Include(s => s.Staff)
                .Where(s => s.Role == "Nurse" &&
                            s.DaysOfWeek.Contains(day) &&
                            s.StartTime <= currentTime &&
                            s.EndTime >= currentTime)
                .ToListAsync();
        }

    }
}
