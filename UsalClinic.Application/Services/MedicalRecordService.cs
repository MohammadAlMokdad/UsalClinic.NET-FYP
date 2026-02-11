using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Core;
using UsalClinic.Core.Entities;

namespace UsalClinic.Application.Services
{
    public class MedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicalRecordService> _logger;

        public MedicalRecordService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MedicalRecordService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MedicalRecordDto> CreateMedicalRecordAsync(MedicalRecordDto medicalRecordDto)
        {
            if (medicalRecordDto == null)
            {
                _logger.LogWarning("Attempted to create a null medical record.");
                throw new ArgumentNullException(nameof(medicalRecordDto));
            }

            _logger.LogInformation("Creating new medical record for Patient ID {PatientId}, Doctor ID {DoctorId}.",
                medicalRecordDto.PatientId, medicalRecordDto.DoctorId);

            var entity = _mapper.Map<MedicalRecord>(medicalRecordDto);
            entity.CreatedAt = DateTime.UtcNow;

            var newEntity = await _unitOfWork.MedicalRecords.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Re-fetch the entity with navigation properties included
            var fullEntity = await _unitOfWork.MedicalRecords.GetMedicalRecordByIdAsync(newEntity.Id);
            if (fullEntity == null)
            {
                _logger.LogError("Created medical record with ID {Id} was not found after saving.", newEntity.Id);
                throw new Exception("Failed to retrieve the created medical record.");
            }

            _logger.LogInformation("Medical record created with ID {Id}.", newEntity.Id);

            // Map the fully loaded entity so DoctorName and PatientName are populated
            return _mapper.Map<MedicalRecordDto>(fullEntity);
        }


        public async Task<MedicalRecordDto?> GetMedicalRecordByIdAsync(int medicalRecordId)
        {
            _logger.LogInformation("Fetching medical record by ID {Id}.", medicalRecordId);
            var record = await _unitOfWork.MedicalRecords.GetMedicalRecordByIdAsync(medicalRecordId);
            if (record == null)
                _logger.LogWarning("Medical record with ID {Id} not found.", medicalRecordId);
            return record == null ? null : _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId)
        {
            _logger.LogInformation("Fetching medical records for patient ID {PatientId}.", patientId);
            var records = await _unitOfWork.MedicalRecords.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<MedicalRecordDto?> GetMedicalRecordByPatientIdAsync(int patientId)
        {
            _logger.LogInformation("Fetching single medical record for patient ID {PatientId}.", patientId);
            var medicalRecord = await _unitOfWork.MedicalRecords.GetMedicalRecordByPatientIdAsync(patientId);
            if (medicalRecord == null)
                _logger.LogWarning("No medical record found for patient ID {PatientId}.", patientId);
            return medicalRecord == null ? null : _mapper.Map<MedicalRecordDto>(medicalRecord);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByDoctorAsync(Guid doctorId)
        {
            _logger.LogInformation("Fetching medical records for doctor ID {DoctorId}.", doctorId);
            var records = await _unitOfWork.MedicalRecords.GetByDoctorIdAsync(doctorId);
            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetAllMedicalRecordsAsync()
        {
            _logger.LogInformation("Fetching all medical records.");
            var records = await _unitOfWork.MedicalRecords.GetAllMedicalRecordsAsync();
            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        public async Task<MedicalRecordDto> UpdateMedicalRecordAsync(MedicalRecordDto medicalRecordDto)
        {
            _logger.LogInformation("Updating medical record with ID {Id}.", medicalRecordDto.Id);
            var existing = await _unitOfWork.MedicalRecords.GetMedicalRecordByIdAsync(medicalRecordDto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Medical record with ID {Id} not found for update.", medicalRecordDto.Id);
                throw new ApplicationException($"MedicalRecord with ID {medicalRecordDto.Id} not found.");
            }
            var originalCreatedAt = existing.CreatedAt;

            _mapper.Map(medicalRecordDto, existing);

            existing.CreatedAt = originalCreatedAt;

            await _unitOfWork.MedicalRecords.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Medical record with ID {Id} updated successfully.", existing.Id);

            return _mapper.Map<MedicalRecordDto>(existing);
        }

        public async Task<bool> DeleteMedicalRecordAsync(int medicalRecordId)
        {
            _logger.LogInformation("Attempting to delete medical record with ID {Id}.", medicalRecordId);
            var existing = await _unitOfWork.MedicalRecords.GetMedicalRecordByIdAsync(medicalRecordId);
            if (existing == null)
            {
                _logger.LogWarning("Medical record with ID {Id} not found for deletion.", medicalRecordId);
                return false;
            }

            await _unitOfWork.MedicalRecords.DeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Medical record with ID {Id} deleted successfully.", medicalRecordId);

            return true;
        }

        public async Task<MedicalRecordDto?> GetMedicalRecordByDoctorAndPatientAsync(Guid doctorId, int patientId)
        {
            _logger.LogInformation("Fetching medical record for doctor ID {DoctorId} and patient ID {PatientId}.", doctorId, patientId);
            var record = await _unitOfWork.MedicalRecords.GetByDoctorAndPatientAsync(doctorId, patientId);
            if (record == null)
                _logger.LogWarning("No record found for doctor ID {DoctorId} and patient ID {PatientId}.", doctorId, patientId);
            return record == null ? null : _mapper.Map<MedicalRecordDto>(record);
        }
    }
}
