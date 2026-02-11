using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;  
using UsalClinic.Core.Entities; 

namespace UsalClinic.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorDto> CreateDoctorAsync(DoctorDto doctorDto);
        Task<DoctorDto> GetDoctorByIdAsync(Guid doctorId);
        Task<IEnumerable<DoctorDto>> GetDoctorsByDepartmentAsync(int departmentId);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> UpdateDoctorAsync(DoctorDto doctorDto);
        Task<bool> DeleteDoctorAsync(Guid doctorId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAsync(Guid doctorId);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByDoctorAsync(Guid doctorId);
    }
}
