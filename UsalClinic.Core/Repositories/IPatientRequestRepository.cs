using AspnetRun.Core.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;

namespace UsalClinic.Core.Repositories
{
    public interface IPatientRequestRepository : IRepository<PatientRequest>
    {
        Task<IEnumerable<PatientRequest>> GetPendingRequestsAsync();
    }
}
