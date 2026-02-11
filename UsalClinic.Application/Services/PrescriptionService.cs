using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using UsalClinic.Application.Models;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;

namespace UsalClinic.Application.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IMapper mapper, ILogger<PrescriptionService> logger)
        {
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<PrescriptionDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all prescriptions.");
            var prescriptions = await _prescriptionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }

        public async Task<PrescriptionDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching prescription by ID: {Id}", id);
            var prescription = await _prescriptionRepository.GetByIdAsync(id);
            if (prescription == null)
            {
                _logger.LogWarning("Prescription with ID {Id} not found.", id);
                return null;
            }
            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task CreateAsync(PrescriptionDto dto)
        {
            _logger.LogInformation("Creating a new prescription for MedicalRecordId: {MedicalRecordId}", dto.MedicalRecordId);
            var entity = _mapper.Map<Prescription>(dto);
            await _prescriptionRepository.AddAsync(entity);
            _logger.LogInformation("Prescription created successfully.");
        }

        public async Task UpdateAsync(PrescriptionDto dto)
        {
            _logger.LogInformation("Updating prescription with ID: {Id}", dto.Id);
            var entity = await _prescriptionRepository.GetByIdAsync(dto.Id);
            if (entity == null)
            {
                _logger.LogWarning("Prescription with ID {Id} not found for update.", dto.Id);
                return;
            }
            var originalCreatedAt = entity.CreatedAt;

            _mapper.Map(dto, entity);
            
            entity.CreatedAt = originalCreatedAt;

            await _prescriptionRepository.UpdateAsync(entity);
            _logger.LogInformation("Prescription with ID {Id} updated successfully.", dto.Id);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting prescription with ID: {Id}", id);
            await _prescriptionRepository.DeleteAsync(id);
            _logger.LogInformation("Prescription with ID {Id} deleted successfully.", id);
        }

        public async Task<IEnumerable<PrescriptionDto>> GetPrescriptionsByMedicalRecordIdAsync(int medicalRecordId)
        {
            _logger.LogInformation("Fetching prescriptions for MedicalRecordId: {MedicalRecordId}", medicalRecordId);
            var prescriptions = await _prescriptionRepository.GetPrescriptionsByMedicalRecordIdAsync(medicalRecordId);
            return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
        }
    }
}
