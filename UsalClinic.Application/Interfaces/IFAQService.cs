using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Core.Entities;

namespace UsalClinic.Application.Interfaces
{
    public interface IFAQEntryService
    {
        Task<FAQEntry> CreateFAQEntryAsync(FAQEntry faqEntry);
        Task<FAQEntry> GetFAQEntryByIdAsync(int faqEntryId);
        Task<IEnumerable<FAQEntry>> GetAllFAQEntriesAsync();
        Task<FAQEntry> UpdateFAQEntryAsync(FAQEntry faqEntry);
        Task<bool> DeleteFAQEntryAsync(int faqEntryId);
        Task CreateFAQEntryAsync(FAQEntryDto fAQEntryDto);
    }
}
