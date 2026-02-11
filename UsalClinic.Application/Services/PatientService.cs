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
    public class PatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientDto> CreatePatientAsync(PatientDto patientDto)
        {
            _logger.LogInformation("Creating patient with UserId: {UserId}", patientDto.UserId);
            await ValidatePatientIfExists(patientDto);

            var entity = _mapper.Map<Patient>(patientDto);
            var result = await _unitOfWork.Patients.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Patient created with ID: {Id}", result.Id);
            return _mapper.Map<PatientDto>(result);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int patientId)
        {
            _logger.LogInformation("Fetching patient with ID: {Id}", patientId);
            var entity = await _unitOfWork.Patients.GetPatientByIdAsync(patientId);
            if (entity == null)
                _logger.LogWarning("Patient with ID: {Id} not found", patientId);
            return entity == null ? null : _mapper.Map<PatientDto>(entity);
        }

        public async Task<PatientDto?> GetPatientByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching patient with UserId: {UserId}", userId);
            var entity = await _unitOfWork.Patients.GetByUserIdAsync(userId);
            if (entity == null)
                _logger.LogWarning("Patient with UserId: {UserId} not found", userId);
            return entity == null ? null : _mapper.Map<PatientDto>(entity);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            _logger.LogInformation("Fetching all patients");
            var entities = await _unitOfWork.Patients.GetAllPatientAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(entities);
        }

        public async Task<PatientDto> UpdatePatientAsync(PatientDto patientDto)
        {
            _logger.LogInformation("Updating patient with ID: {Id}", patientDto.Id);
            var existing = await _unitOfWork.Patients.GetPatientByIdAsync(patientDto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Patient with ID {Id} not found for update", patientDto.Id);
                throw new ApplicationException($"Patient with ID {patientDto.Id} does not exist.");
            }

            _mapper.Map(patientDto, existing);
            await _unitOfWork.Patients.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Patient with ID: {Id} updated successfully", patientDto.Id);
            return _mapper.Map<PatientDto>(existing);
        }

        public async Task<bool> DeletePatientAsync(int patientId)
        {
            _logger.LogInformation("Deleting patient with ID: {Id}", patientId);
            var existing = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (existing == null)
            {
                _logger.LogWarning("Patient with ID: {Id} not found for deletion", patientId);
                return false;
            }

            await _unitOfWork.Patients.DeleteAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Patient with ID: {Id} deleted successfully", patientId);
            return true;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(int patientId)
        {
            _logger.LogInformation("Fetching appointments for patient ID: {PatientId}", patientId);
            var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(int patientId)
        {
            _logger.LogInformation("Fetching medical records for patient ID: {PatientId}", patientId);
            var records = await _unitOfWork.MedicalRecords.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<MedicalRecordDto>>(records);
        }

        private async Task ValidatePatientIfExists(PatientDto patientDto)
        {
            _logger.LogInformation("Validating if patient already exists for UserId: {UserId}", patientDto.UserId);
            var existing = await _unitOfWork.Patients.GetByUserIdAsync(patientDto.UserId);
            if (existing != null)
            {
                _logger.LogWarning("Patient already exists for UserId: {UserId}", patientDto.UserId);
                throw new ApplicationException($"Patient with UserId {patientDto.UserId} already exists.");
            }
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsByDoctorUserIdAsync(string doctorUserId)
        {
            _logger.LogInformation("Fetching patients assigned to doctor with UserId: {DoctorUserId}", doctorUserId);
            var patients = await _unitOfWork.Patients.GetPatientsByDoctorUserIdAsync(doctorUserId);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
    }
}
