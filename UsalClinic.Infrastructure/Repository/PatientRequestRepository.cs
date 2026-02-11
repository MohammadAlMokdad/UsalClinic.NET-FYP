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
    public class PatientRequestRepository : Repository<PatientRequest>,IPatientRequestRepository
    {
        public PatientRequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<PatientRequest>> GetPendingRequestsAsync()
        {
            return await _dbContext.PatientRequests
                .Where(r => !r.IsApproved && !r.IsRejected)
                .ToListAsync();
        }

    }
}
