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
    public class NurseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NurseService> _logger;

        public NurseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<NurseService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<NurseDto>> GetAllNursesAsync()
        {
            _logger.LogInformation("Retrieving all nurses.");
            var nurses = await _unitOfWork.Nurses.GetAllAsync();
            return _mapper.Map<List<NurseDto>>(nurses);
        }

        public async Task<NurseDto?> GetNurseByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving nurse with ID {NurseId}.", id);
            var nurse = await _unitOfWork.Nurses.GetByIdAsync(id);
            if (nurse == null)
            {
                _logger.LogWarning("Nurse with ID {NurseId} not found.", id);
                return null;
            }

            return _mapper.Map<NurseDto>(nurse);
        }

        public async Task<NurseDto> CreateNurseAsync(NurseDto nurseDto)
        {
            var nurse = _mapper.Map<Nurse>(nurseDto);
            nurse.Id = Guid.NewGuid();
            nurse.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Creating new nurse with ID {NurseId}.", nurse.Id);

            try
            {
                await _unitOfWork.Nurses.AddAsync(nurse);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Nurse created successfully with ID {NurseId}.", nurse.Id);
                return _mapper.Map<NurseDto>(nurse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating nurse.");
                throw;
            }
        }

        public async Task<NurseDto?> UpdateNurseAsync(Guid id, NurseDto nurseDto)
        {
            _logger.LogInformation("Updating nurse with ID {NurseId}.", id);
            var existingNurse = await _unitOfWork.Nurses.GetByIdAsync(id);
            if (existingNurse == null)
            {
                _logger.LogWarning("Nurse with ID {NurseId} not found for update.", id);
                return null;
            }
            var originalCreatedAt = existingNurse.CreatedAt;

            _mapper.Map(nurseDto, existingNurse);

            existingNurse.CreatedAt = originalCreatedAt;

            await _unitOfWork.Nurses.UpdateAsync(existingNurse);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Nurse with ID {NurseId} updated successfully.", id);
            return _mapper.Map<NurseDto>(existingNurse);
        }

        public async Task<bool> DeleteNurseAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete nurse with ID {NurseId}.", id);
            var nurse = await _unitOfWork.Nurses.GetByIdAsync(id);
            if (nurse == null)
            {
                _logger.LogWarning("Nurse with ID {NurseId} not found for deletion.", id);
                return false;
            }

            await _unitOfWork.Nurses.DeleteAsync(nurse);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Nurse with ID {NurseId} deleted successfully.", id);
            return true;
        }

        public async Task<NurseDto?> GetNurseByUserIdAsync(string userId)
        {
            _logger.LogInformation("Retrieving nurse by UserId {UserId}.", userId);
            var nurse = await _unitOfWork.Nurses.GetByUserIdAsync(userId);
            if (nurse == null)
            {
                _logger.LogWarning("Nurse with UserId {UserId} not found.", userId);
                return null;
            }

            return _mapper.Map<NurseDto>(nurse);
        }
        public async Task<NurseDto> UpdateNurseAsync(NurseDto nurseDto)
        {
            _logger.LogInformation("Updating nurse with ID {NurseId}.", nurseDto.Id);

            var existing = await _unitOfWork.Nurses.GetByIdAsync(nurseDto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Nurse with ID {NurseId} not found for update.", nurseDto.Id);
                throw new ApplicationException($"Nurse with ID {nurseDto.Id} does not exist.");
            }

            _mapper.Map(nurseDto, existing);
            await _unitOfWork.Nurses.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Nurse with ID {NurseId} updated successfully.", nurseDto.Id);
            return _mapper.Map<NurseDto>(existing);
        }

    }
}
