using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure.Repository.Base;
using UsalClinic.Web.Data;

namespace UsalClinic.Infrastructure.Repository
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Patient?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.Patients
                  .FirstOrDefaultAsync(p => p.UserId == userId);
        }
        public async Task<IEnumerable<Patient>> GetAllPatientAsync()
        {
            return await _dbContext.Patients
                .Include(p => p.User)
                .ToListAsync();
        }
        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _dbContext.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Patient>> GetPatientsByDoctorUserIdAsync(string doctorUserId)
        {
            return await _dbContext.Patients
                .Include(p => p.User)
                .Include(p => p.MedicalRecords)
                .Include(p => p.Appointments)
                .Where(p =>
                    p.MedicalRecords.Any(r => r.Doctor.UserId == doctorUserId) ||
                    p.Appointments.Any(a => a.Doctor.UserId == doctorUserId))
                .ToListAsync();
        }
    }
}
