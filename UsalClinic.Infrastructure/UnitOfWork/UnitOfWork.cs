using System.Threading.Tasks;
using UsalClinic.Core;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure.Repository;
using UsalClinic.Web.Data;

namespace UsalClinic.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Appointments = new AppointmentRepository(_context);
            Departments = new DepartmentRepository(_context);
            Doctors = new DoctorRepository(_context);
            MedicalRecords = new MedicalRecordRepository(_context);
            Patients = new PatientRepository(_context);
            Prescriptions = new PrescriptionRepository(_context);
            Rooms = new RoomRepository(_context);
            FAQs = new FAQEntryRepository(_context);
            AuditLogs = new AuditLogRepository(_context);
            Nurses = new NurseRepository(_context);
            Shifts = new ShiftRepository(_context);
            PatientRequests = new PatientRequestRepository(context);
        }

        public IAppointmentRepository Appointments { get; }
        public IDepartmentRepository Departments { get; }
        public IDoctorRepository Doctors { get; }
        public IMedicalRecordRepository MedicalRecords { get; }
        public IPatientRepository Patients { get; }
        public IPrescriptionRepository Prescriptions { get; }
        public IRoomRepository Rooms { get; }
        public IFAQEntryRepository FAQs { get; }
        public IAuditLogRepository AuditLogs { get; }
        public INurseRepository Nurses { get; }
        public IShiftRepository Shifts { get; }
        public IPatientRequestRepository PatientRequests { get; private set; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
