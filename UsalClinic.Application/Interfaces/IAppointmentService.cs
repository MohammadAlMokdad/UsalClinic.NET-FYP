using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
namespace UsalClinic.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto appointmentDto);
        Task<AppointmentDto> GetAppointmentByIdAsync(int appointmentId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAsync(Guid doctorId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto> UpdateAppointmentAsync(AppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
    }
}
