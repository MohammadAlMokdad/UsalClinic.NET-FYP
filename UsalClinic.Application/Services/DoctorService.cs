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
    public class DoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DoctorService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<DoctorDto>> GetAllDoctorsAsync()
        {
            _logger.LogInformation("Retrieving all doctors.");
            var doctors = await _unitOfWork.Doctors.GetAllDoctorsAsync();
            return _mapper.Map<List<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving doctor with ID {DoctorId}.", id);
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found.", id);
                return null;
            }

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> CreateDoctorAsync(DoctorDto doctorDto)
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            doctor.Id = Guid.NewGuid();
            _logger.LogInformation("Creating new doctor with ID {DoctorId}.", doctor.Id);

            try
            {
                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Doctor created successfully with ID {DoctorId}.", doctor.Id);
                return _mapper.Map<DoctorDto>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating doctor.");
                throw;
            }
        }

        public async Task<DoctorDto?> UpdateDoctorGuidAsync(Guid id, DoctorDto doctorDto)
        {
            _logger.LogInformation("Updating doctor with ID {DoctorId}.", id);
            var existingDoctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(id);
            if (existingDoctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found for update.", id);
                return null;
            }

            _mapper.Map(doctorDto, existingDoctor);
            await _unitOfWork.Doctors.UpdateAsync(existingDoctor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Doctor with ID {DoctorId} updated successfully.", id);
            return _mapper.Map<DoctorDto>(existingDoctor);
        }

        public async Task<DoctorDto> UpdateDoctorAsync(DoctorDto doctorDto)
        {
            _logger.LogInformation("Updating doctor using DTO with ID {DoctorId}.", doctorDto.Id);
            var existing = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorDto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found for update.", doctorDto.Id);
                throw new ApplicationException($"Doctor with ID {doctorDto.Id} does not exist.");
            }

            _mapper.Map(doctorDto, existing);
            await _unitOfWork.Doctors.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Doctor with ID {DoctorId} updated successfully.", doctorDto.Id);
            return _mapper.Map<DoctorDto>(existing);
        }

        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete doctor with ID {DoctorId}.", id);
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with ID {DoctorId} not found for deletion.", id);
                return false;
            }

            await _unitOfWork.Doctors.DeleteAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Doctor with ID {DoctorId} deleted successfully.", id);
            return true;
        }

        public async Task<DoctorDto?> GetDoctorByUserIdAsync(string userId)
        {
            _logger.LogInformation("Retrieving doctor by UserId {UserId}.", userId);
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor with UserId {UserId} not found.", userId);
                return null;
            }

            return _mapper.Map<DoctorDto>(doctor);
        }
    }
}
