using System;
using System.Threading.Tasks;
using UsalClinic.Core.Repositories;

namespace UsalClinic.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IAppointmentRepository Appointments { get; }
        IDepartmentRepository Departments { get; }
        IDoctorRepository Doctors { get; }
        IMedicalRecordRepository MedicalRecords { get; }
        IPatientRepository Patients { get; }
        IPrescriptionRepository Prescriptions { get; }
        IRoomRepository Rooms { get; }
        IFAQEntryRepository FAQs { get; }
        IAuditLogRepository AuditLogs { get; }
        INurseRepository Nurses { get; }
        IShiftRepository Shifts { get; }
        IPatientRequestRepository PatientRequests { get; }
        Task<int> SaveChangesAsync();
    }
}
