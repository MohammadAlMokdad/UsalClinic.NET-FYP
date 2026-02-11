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
    public class PatientServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _mockLogger = new Mock<ILogger<PatientService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new PatientService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreatePatientAsync_ThrowsException_IfPatientExists()
        {
            var dto = new PatientDto { UserId = "user123" };
            _mockUnitOfWork.Setup(u => u.Patients.GetByUserIdAsync(dto.UserId)).ReturnsAsync(new Patient());

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.CreatePatientAsync(dto));
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public async Task CreatePatientAsync_Success()
        {
            var dto = new PatientDto { UserId = "user123" };
            var entity = new Patient();
            var savedEntity = new Patient { Id = 1 };

            _mockUnitOfWork.Setup(u => u.Patients.GetByUserIdAsync(dto.UserId)).ReturnsAsync((Patient?)null);
            _mockMapper.Setup(m => m.Map<Patient>(dto)).Returns(entity);
            _mockUnitOfWork.Setup(u => u.Patients.AddAsync(entity)).ReturnsAsync(savedEntity);
            _mockMapper.Setup(m => m.Map<PatientDto>(savedEntity)).Returns(dto);

            var result = await _service.CreatePatientAsync(dto);

            _mockUnitOfWork.Verify(u => u.Patients.AddAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal(dto.UserId, result.UserId);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ReturnsDto_IfExists()
        {
            var patient = new Patient { Id = 1 };
            var dto = new PatientDto { Id = 1 };

            _mockUnitOfWork.Setup(u => u.Patients.GetPatientByIdAsync(1)).ReturnsAsync(patient);
            _mockMapper.Setup(m => m.Map<PatientDto>(patient)).Returns(dto);

            var result = await _service.GetPatientByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.Id);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ReturnsNull_IfNotFound()
        {
            _mockUnitOfWork.Setup(u => u.Patients.GetPatientByIdAsync(It.IsAny<int>())).ReturnsAsync((Patient?)null);

            var result = await _service.GetPatientByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdatePatientAsync_ThrowsException_IfNotFound()
        {
            var dto = new PatientDto { Id = 1 };
            _mockUnitOfWork.Setup(u => u.Patients.GetPatientByIdAsync(dto.Id)).ReturnsAsync((Patient?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.UpdatePatientAsync(dto));
            Assert.Contains("does not exist", ex.Message);
        }

        [Fact]
        public async Task UpdatePatientAsync_Success()
        {
            var dto = new PatientDto { Id = 1 };
            var existing = new Patient { Id = 1 };

            _mockUnitOfWork.Setup(u => u.Patients.GetPatientByIdAsync(dto.Id)).ReturnsAsync(existing);

            _mockMapper.Setup(m => m.Map(dto, existing));
            _mockMapper.Setup(m => m.Map<PatientDto>(existing)).Returns(dto);

            var result = await _service.UpdatePatientAsync(dto);

            _mockUnitOfWork.Verify(u => u.Patients.UpdateAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task DeletePatientAsync_ReturnsFalse_IfNotFound()
        {
            _mockUnitOfWork.Setup(u => u.Patients.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Patient?)null);

            var result = await _service.DeletePatientAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task DeletePatientAsync_ReturnsTrue_IfDeleted()
        {
            var existing = new Patient { Id = 1 };
            _mockUnitOfWork.Setup(u => u.Patients.GetByIdAsync(1)).ReturnsAsync(existing);

            var result = await _service.DeletePatientAsync(1);

            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Patients.DeleteAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentsByPatientAsync_ReturnsMappedList()
        {
            var appointments = new List<Appointment> { new Appointment { Id = 1 } };
            var dtos = new List<AppointmentDto> { new AppointmentDto { Id = 1 } };

            _mockUnitOfWork.Setup(u => u.Appointments.GetByPatientIdAsync(1)).ReturnsAsync(appointments);
            _mockMapper.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments)).Returns(dtos);

            var result = await _service.GetAppointmentsByPatientAsync(1);

            Assert.NotEmpty(result);
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
        public async Task GetPatientsByDoctorUserIdAsync_ReturnsMappedList()
        {
            var patients = new List<Patient> { new Patient { Id = 1 } };
            var dtos = new List<PatientDto> { new PatientDto { Id = 1 } };

            _mockUnitOfWork.Setup(u => u.Patients.GetPatientsByDoctorUserIdAsync("doctorUserId")).ReturnsAsync(patients);
            _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(patients)).Returns(dtos);

            var result = await _service.GetPatientsByDoctorUserIdAsync("doctorUserId");

            Assert.NotEmpty(result);
        }
    }
}
