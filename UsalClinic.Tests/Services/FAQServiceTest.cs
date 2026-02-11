using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core;
using UsalClinic.Core.Entities;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class FAQEntryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<FAQEntryService>> _mockLogger;
        private readonly FAQEntryService _service;

        public FAQEntryServiceTests()
        {
            _mockLogger = new Mock<ILogger<FAQEntryService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new FAQEntryService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllFAQEntriesAsync_ReturnsMappedList()
        {
            var faqEntries = new List<FAQEntry>
            {
                new FAQEntry { Id = 1, Question = "Q1", Answer = "A1" },
                new FAQEntry { Id = 2, Question = "Q2", Answer = "A2" }
            };
            var dtoList = new List<FAQEntryDto>
            {
                new FAQEntryDto { Id = 1, Question = "Q1", Answer = "A1" },
                new FAQEntryDto { Id = 2, Question = "Q2", Answer = "A2" }
            };

            _mockUnitOfWork.Setup(u => u.FAQs.GetAllAsync()).ReturnsAsync(faqEntries);
            _mockMapper.Setup(m => m.Map<List<FAQEntryDto>>(faqEntries)).Returns(dtoList);

            var result = await _service.GetAllFAQEntriesAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Q1", result[0].Question);
        }

        [Fact]
        public async Task GetFAQEntryByIdAsync_ExistingId_ReturnsMappedDto()
        {
            var faq = new FAQEntry { Id = 1, Question = "Q1", Answer = "A1" };
            var dto = new FAQEntryDto { Id = 1, Question = "Q1", Answer = "A1" };

            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(1)).ReturnsAsync(faq);
            _mockMapper.Setup(m => m.Map<FAQEntryDto>(faq)).Returns(dto);

            var result = await _service.GetFAQEntryByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Q1", result?.Question);
        }

        [Fact]
        public async Task GetFAQEntryByIdAsync_NonExistingId_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((FAQEntry?)null);

            var result = await _service.GetFAQEntryByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateFAQEntryAsync_AddsAndReturnsDto()
        {
            var dto = new FAQEntryDto { Question = "New Q", Answer = "New A" };
            var entity = new FAQEntry { Question = "New Q", Answer = "New A" };
            var returnedDto = new FAQEntryDto { Id = 1, Question = "New Q", Answer = "New A" };

            _mockMapper.Setup(m => m.Map<FAQEntry>(dto)).Returns(entity);
            _mockMapper.Setup(m => m.Map<FAQEntryDto>(entity)).Returns(returnedDto);

            var result = await _service.CreateFAQEntryAsync(dto);

            _mockUnitOfWork.Verify(u => u.FAQs.AddAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal("New Q", result.Question);
        }

        [Fact]
        public async Task UpdateFAQEntryAsync_ExistingEntry_UpdatesAndReturnsDto()
        {
            var id = 1;
            var existing = new FAQEntry { Id = id, Question = "Old Q", Answer = "Old A" };
            var dto = new FAQEntryDto { Id = id, Question = "Updated Q", Answer = "Updated A" };

            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(id)).ReturnsAsync(existing);
            _mockMapper.Setup(m => m.Map(dto, existing));
            _mockMapper.Setup(m => m.Map<FAQEntryDto>(existing)).Returns(dto);

            var result = await _service.UpdateFAQEntryAsync(id, dto);

            Assert.NotNull(result);
            Assert.Equal("Updated Q", result.Question);
        }

        [Fact]
        public async Task UpdateFAQEntryAsync_NonExistingEntry_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((FAQEntry?)null);

            var result = await _service.UpdateFAQEntryAsync(999, new FAQEntryDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteFAQEntryAsync_ExistingEntry_ReturnsTrue()
        {
            var faq = new FAQEntry { Id = 1, Question = "Q", Answer = "A" };

            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(1)).ReturnsAsync(faq);

            var result = await _service.DeleteFAQEntryAsync(1);

            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.FAQs.DeleteAsync(faq), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteFAQEntryAsync_NonExistingEntry_ReturnsFalse()
        {
            _mockUnitOfWork.Setup(u => u.FAQs.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((FAQEntry?)null);

            var result = await _service.DeleteFAQEntryAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateFAQEntryAsync_WithDto_CallsUpdate()
        {
            var dto = new FAQEntryDto { Id = 1, Question = "Q", Answer = "A" };
            var entity = new FAQEntry { Id = 1, Question = "Q", Answer = "A" };

            _mockMapper.Setup(m => m.Map<FAQEntry>(dto)).Returns(entity);

            await _service.UpdateFAQEntryAsync(dto);

            _mockUnitOfWork.Verify(u => u.FAQs.UpdateAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
