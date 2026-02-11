using UsalClinic.Core.Entities;
using AspnetRun.Core.Repositories.Base;

namespace UsalClinic.Core.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        //Appointment GetById(int id);
        Task DeleteAsync(int id);
        Task<IReadOnlyList<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    }
}
