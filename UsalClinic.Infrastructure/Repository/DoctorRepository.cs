using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;
using UsalClinic.Infrastructure.Repository.Base;
using UsalClinic.Web.Data;

namespace UsalClinic.Infrastructure.Repository
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IReadOnlyList<Doctor>> GetAllDoctorsAsync()
        {
            return await _dbContext.Doctors
                .Include(p => p.User)
                .ToListAsync();
        }
        public async Task<Doctor?> GetDoctorByIdAsync(Guid id)
        {
            return await _dbContext.Doctors
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Doctor?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
