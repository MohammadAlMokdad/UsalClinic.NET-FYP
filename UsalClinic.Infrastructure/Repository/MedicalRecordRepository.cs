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
    public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _dbContext.MedicalRecords
                                            .Where(mr => mr.DoctorId == doctorId)
                                            .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Doctor)
                    .ThenInclude(d => d.User)
                .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetAllMedicalRecordsAsync()
        {
            return await _dbContext.MedicalRecords
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id)
        {
            return await _dbContext.MedicalRecords
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<MedicalRecord?> GetMedicalRecordByPatientIdAsync(int patientId)
        {
            return await _dbContext.MedicalRecords
                .Include(m => m.Doctor)
                    .ThenInclude(d => d.User)
                .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientId == patientId);
        }
        public async Task<MedicalRecord?> GetByDoctorAndPatientAsync(Guid doctorId, int patientId)
        {
            return await _dbContext.MedicalRecords
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.DoctorId == doctorId && r.PatientId == patientId);
        }

    }
}
