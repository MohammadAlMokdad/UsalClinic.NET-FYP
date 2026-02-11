using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure.Repository.Base;
using UsalClinic.Web.Data;

namespace UsalClinic.Infrastructure.Repository
{
    public class DoctorDepartmentRepository : Repository<DoctorDepartment>, IDoctorDepartmentRepository
    {
        public DoctorDepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
