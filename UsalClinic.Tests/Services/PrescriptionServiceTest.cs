using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<IPrescriptionRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PrescriptionService>> _mockLogger;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTests()
        {
            _mockRepo = new Mock<IPrescriptionRepository>();
            _mockLogger = new Mock<ILogger<PrescriptionService>>();
            _mockMapper = new Mock<IMapper>();
            _service = new PrescriptionService(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedList()
        {
            var prescriptions = new List<Prescription> { new Prescription { Id = 1 } };
            var dtos = new List<PrescriptionDto> { new PrescriptionDto { Id = 1 } };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(prescriptions);
            _mockMapper.Setup(m => m.Map<IEnumerable<PrescriptionDto>>(prescriptions)).Returns(dtos);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ReturnsDto()
        {
            var prescription = new Prescription { Id = 1 };
            var dto = new PrescriptionDto { Id = 1 };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(prescription);
            _mockMapper.Setup(m => m.Map<PrescriptionDto>(prescription)).Returns(dto);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task GetByIdAsync_NonExisting_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Prescription?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_CallsAddAsync()
        {
            var dto = new PrescriptionDto { Id = 0 };
            var entity = new Prescription();

            _mockMapper.Setup(m => m.Map<Prescription>(dto)).Returns(entity);

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(entity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Existing_MapsAndUpdates()
        {
            var dto = new PrescriptionDto { Id = 1 };
            var entity = new Prescription { Id = 1 };

            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(entity);

            await _service.UpdateAsync(dto);

            _mockMapper.Verify(m => m.Map(dto, entity), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(entity), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NonExisting_DoesNothing()
        {
            var dto = new PrescriptionDto { Id = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Prescription?)null);

            await _service.UpdateAsync(dto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Prescription>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_CallsDelete()
        {
            await _service.DeleteAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetPrescriptionsByMedicalRecordIdAsync_ReturnsMappedList()
        {
            var prescriptions = new List<Prescription> { new Prescription { Id = 1 } };
            var dtos = new List<PrescriptionDto> { new PrescriptionDto { Id = 1 } };

            _mockRepo.Setup(r => r.GetPrescriptionsByMedicalRecordIdAsync(1)).ReturnsAsync(prescriptions);
            _mockMapper.Setup(m => m.Map<IEnumerable<PrescriptionDto>>(prescriptions)).Returns(dtos);

            var result = await _service.GetPrescriptionsByMedicalRecordIdAsync(1);

            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
