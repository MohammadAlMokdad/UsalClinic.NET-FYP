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
    public class MedicalRecordServiceTests
    {

        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<MedicalRecordService>> _mockLogger;
        private readonly MedicalRecordService _service;

        public MedicalRecordServiceTests()
        {
            _mockLogger = new Mock<ILogger<MedicalRecordService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new MedicalRecordService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateMedicalRecordAsync_NullDto_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateMedicalRecordAsync(null!));
        }

        [Fact]
        public async Task CreateMedicalRecordAsync_ValidDto_AddsAndReturnsDto()
        {
            var dto = new MedicalRecordDto { Id = 0, PatientId = 1, DoctorId = Guid.NewGuid() };
            var entity = new MedicalRecord();
            var returnedEntity = new MedicalRecord { Id = 1 };

            _mockMapper.Setup(m => m.Map<MedicalRecord>(dto)).Returns(entity);
            _mockUnitOfWork.Setup(u => u.MedicalRecords.AddAsync(entity)).ReturnsAsync(returnedEntity);
            _mockMapper.Setup(m => m.Map<MedicalRecordDto>(returnedEntity)).Returns(new MedicalRecordDto { Id = 1 });

            var result = await _service.CreateMedicalRecordAsync(dto);

            _mockUnitOfWork.Verify(u => u.MedicalRecords.AddAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetMedicalRecordByIdAsync_ExistingId_ReturnsDto()
        {
            var record = new MedicalRecord { Id = 1 };
            var dto = new MedicalRecordDto { Id = 1 };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(1)).ReturnsAsync(record);
            _mockMapper.Setup(m => m.Map<MedicalRecordDto>(record)).Returns(dto);

            var result = await _service.GetMedicalRecordByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task GetMedicalRecordByIdAsync_NonExistingId_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MedicalRecord?)null);

            var result = await _service.GetMedicalRecordByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetMedicalRecordsByPatientAsync_ReturnsMappedList()
        {
            var records = new List<MedicalRecord> { new MedicalRecord { Id = 1 } };
            var dtos = new List<MedicalRecordDto> { new MedicalRecordDto { Id = 1 } };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetByPatientIdAsync(1)).ReturnsAsync(records);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicalRecordDto>>(records)).Returns(dtos);

            var result = await _service.GetMedicalRecordsByPatientAsync(1);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_ExistingRecord_UpdatesAndReturnsDto()
        {
            var dto = new MedicalRecordDto { Id = 1 };
            var existing = new MedicalRecord { Id = 1 };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(1)).ReturnsAsync(existing);
            _mockMapper.Setup(m => m.Map(dto, existing));
            _mockMapper.Setup(m => m.Map<MedicalRecordDto>(existing)).Returns(dto);

            var result = await _service.UpdateMedicalRecordAsync(dto);

            _mockUnitOfWork.Verify(u => u.MedicalRecords.UpdateAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task UpdateMedicalRecordAsync_NonExistingRecord_ThrowsException()
        {
            var dto = new MedicalRecordDto { Id = 1 };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(dto.Id))
                .ReturnsAsync((MedicalRecord?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.UpdateMedicalRecordAsync(dto));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task DeleteMedicalRecordAsync_ExistingRecord_ReturnsTrue()
        {
            var existing = new MedicalRecord { Id = 1 };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(1)).ReturnsAsync(existing);

            var result = await _service.DeleteMedicalRecordAsync(1);

            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.MedicalRecords.DeleteAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicalRecordAsync_NonExistingRecord_ReturnsFalse()
        {
            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetMedicalRecordByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MedicalRecord?)null);

            var result = await _service.DeleteMedicalRecordAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task GetMedicalRecordByDoctorAndPatientAsync_Existing_ReturnsDto()
        {
            var doctorId = Guid.NewGuid();
            var patientId = 1;
            var record = new MedicalRecord { Id = 1 };
            var dto = new MedicalRecordDto { Id = 1 };

            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetByDoctorAndPatientAsync(doctorId, patientId)).ReturnsAsync(record);
            _mockMapper.Setup(m => m.Map<MedicalRecordDto>(record)).Returns(dto);

            var result = await _service.GetMedicalRecordByDoctorAndPatientAsync(doctorId, patientId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetMedicalRecordByDoctorAndPatientAsync_NonExisting_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.MedicalRecords.GetByDoctorAndPatientAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync((MedicalRecord?)null);

            var result = await _service.GetMedicalRecordByDoctorAndPatientAsync(Guid.NewGuid(), 999);

            Assert.Null(result);
        }
    }
}
