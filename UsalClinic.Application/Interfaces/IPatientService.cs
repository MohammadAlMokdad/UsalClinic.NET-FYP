using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;  

namespace UsalClinic.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto> CreatePatientAsync(PatientDto patientDto);
        Task<PatientDto> GetPatientByIdAsync(int patientId);
        Task<PatientDto> GetPatientByUserIdAsync(string userId); 
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> UpdatePatientAsync(PatientDto patientDto);
        Task<bool> DeletePatientAsync(int patientId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId);
    }
}
