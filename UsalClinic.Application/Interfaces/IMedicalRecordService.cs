using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;  

namespace UsalClinic.Application.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> CreateMedicalRecordAsync(MedicalRecordDto medicalRecordDto);
        Task<MedicalRecordDto> GetMedicalRecordByIdAsync(int medicalRecordId);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId);
        Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByDoctorAsync(Guid doctorId);
        Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync();
        Task<MedicalRecordDto> UpdateMedicalRecordAsync(MedicalRecordDto medicalRecordDto);
        Task<bool> DeleteMedicalRecordAsync(int medicalRecordId);
    }
}
