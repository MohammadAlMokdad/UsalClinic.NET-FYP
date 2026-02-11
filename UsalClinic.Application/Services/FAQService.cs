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
    public class FAQEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FAQEntryService> _logger;

        public FAQEntryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FAQEntryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<FAQEntryDto>> GetAllFAQEntriesAsync()
        {
            _logger.LogInformation("Fetching all FAQ entries.");
            var faqEntries = await _unitOfWork.FAQs.GetAllAsync();
            return _mapper.Map<List<FAQEntryDto>>(faqEntries);
        }

        public async Task<FAQEntryDto?> GetFAQEntryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching FAQ entry with ID {FaqId}.", id);
            var faqEntry = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faqEntry == null)
            {
                _logger.LogWarning("FAQ entry with ID {FaqId} not found.", id);
                return null;
            }

            return _mapper.Map<FAQEntryDto>(faqEntry);
        }

        public async Task<FAQEntryDto> CreateFAQEntryAsync(FAQEntryDto faqEntryDto)
        {
            var faqEntry = _mapper.Map<FAQEntry>(faqEntryDto);
            faqEntry.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Creating new FAQ entry: {Question}", faqEntry.Question);

            try
            {
                await _unitOfWork.FAQs.AddAsync(faqEntry);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("FAQ entry created successfully with ID {FaqId}.", faqEntry.Id);
                return _mapper.Map<FAQEntryDto>(faqEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating FAQ entry.");
                throw;
            }
        }

        public async Task<FAQEntryDto?> UpdateFAQEntryAsync(int id, FAQEntryDto faqEntryDto)
        {
            _logger.LogInformation("Updating FAQ entry with ID {FaqId}.", id);
            var existingFAQEntry = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (existingFAQEntry == null)
            {
                _logger.LogWarning("FAQ entry with ID {FaqId} not found for update.", id);
                return null;
            }

            _mapper.Map(faqEntryDto, existingFAQEntry);
            await _unitOfWork.FAQs.UpdateAsync(existingFAQEntry);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("FAQ entry with ID {FaqId} updated successfully.", id);
            return _mapper.Map<FAQEntryDto>(existingFAQEntry);
        }

        public async Task<bool> DeleteFAQEntryAsync(int id)
        {
            _logger.LogInformation("Deleting FAQ entry with ID {FaqId}.", id);
            var faqEntry = await _unitOfWork.FAQs.GetByIdAsync(id);
            if (faqEntry == null)
            {
                _logger.LogWarning("FAQ entry with ID {FaqId} not found for deletion.", id);
                return false;
            }

            try
            {
                await _unitOfWork.FAQs.DeleteAsync(faqEntry);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("FAQ entry with ID {FaqId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting FAQ entry with ID {FaqId}.", id);
                throw;
            }
        }

        public async Task UpdateFAQEntryAsync(FAQEntryDto dto)
        {
            _logger.LogInformation("Performing alternate update for FAQ entry with ID {FaqId}.", dto.Id);

            var existingFAQ = await _unitOfWork.FAQs.GetByIdAsync(dto.Id);
            if (existingFAQ == null)
            {
                _logger.LogWarning("FAQ entry with ID {FaqId} not found for update.", dto.Id);
                throw new ApplicationException($"FAQ entry with Id {dto.Id} not found.");
            }

            var originalCreatedAt = existingFAQ.CreatedAt;

            _mapper.Map(dto, existingFAQ);

            existingFAQ.CreatedAt = originalCreatedAt;

            await _unitOfWork.FAQs.UpdateAsync(existingFAQ);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("FAQ entry with ID {FaqId} updated successfully (alternate method).", dto.Id);
        }

    }
}
