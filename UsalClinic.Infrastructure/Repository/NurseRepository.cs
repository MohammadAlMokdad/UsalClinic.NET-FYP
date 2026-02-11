using Microsoft.EntityFrameworkCore;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure.Repository.Base;
using UsalClinic.Web.Data;

namespace UsalClinic.Infrastructure.Repository
{
    public class NurseRepository : Repository<Nurse>, INurseRepository
    {
        public NurseRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Implement custom Nurse methods if needed
        public async Task<IReadOnlyList<Nurse>> GetAllAsync()
        {
            return await _dbContext.Nurses
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<Nurse?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Nurses
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Nurse?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Nurses
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.UserId == userId);
        }
    }
}
