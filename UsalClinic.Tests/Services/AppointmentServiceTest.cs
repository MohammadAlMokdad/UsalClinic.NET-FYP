using AutoMapper;
using Microsoft.Extensions.Logging;  
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core;
using UsalClinic.Core.Entities;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AppointmentService>> _mockLogger;
        private readonly AppointmentService _service;
        private readonly Mock<IEmailService> _mockEmailService;

        public AppointmentServiceTests()
        {
            _mockLogger = new Mock<ILogger<AppointmentService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockEmailService = new Mock<IEmailService>();
            _service = new AppointmentService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ReturnsMappedAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment { Id = 1, PatientId = 101 },
                new Appointment { Id = 2, PatientId = 102 }
            };
            var appointmentDtos = new List<AppointmentDto>
            {
                new AppointmentDto { Id = 1, PatientId = 101 },
                new AppointmentDto { Id = 2, PatientId = 102 }
            };

            _mockUnitOfWork.Setup(u => u.Appointments.GetAllAppointmentsAsync())
                           .ReturnsAsync(appointments);
            _mockMapper.Setup(m => m.Map<IEnumerable<AppointmentDto>>(appointments))
                       .Returns(appointmentDtos);
            _mockEmailService
                    .Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            // Act
            var result = await _service.GetAllAppointmentsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ExistingId_ReturnsMappedDto()
        {
            // Arrange
            var appointment = new Appointment { Id = 1, PatientId = 101 };
            var appointmentDto = new AppointmentDto { Id = 1, PatientId = 101 };

            _mockUnitOfWork.Setup(u => u.Appointments.GetAppointmentByIdAsync(1))
                           .ReturnsAsync(appointment);
            _mockMapper.Setup(m => m.Map<AppointmentDto>(appointment))
                       .Returns(appointmentDto);

            // Act
            var result = await _service.GetAppointmentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddAppointmentAsync_ValidDto_CallsAddAndSave()
        {
            // Arrange
            var dto = new AppointmentDto { Id = 1 };
            var entity = new Appointment { Id = 1 };

            _mockMapper.Setup(m => m.Map<Appointment>(dto)).Returns(entity);

            // Act
            await _service.AddAppointmentAsync(dto);

            // Assert
            _mockUnitOfWork.Verify(u => u.Appointments.AddAsync(It.Is<Appointment>(a => a.Id == 1)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_ExistingAppointment_CallsUpdateAndSave()
        {
            // Arrange
            var dto = new AppointmentDto { Id = 1, PatientId = 200 };
            var existing = new Appointment { Id = 1, PatientId = 100 };

            _mockUnitOfWork.Setup(u => u.Appointments.GetAppointmentByIdAsync(1))
                           .ReturnsAsync(existing);

            // Act
            await _service.UpdateAppointmentAsync(dto);

            // Assert
            _mockMapper.Verify(m => m.Map(dto, existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.Appointments.UpdateAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_CallsDeleteAndSave()
        {
            // Act
            await _service.DeleteAppointmentAsync(1);

            // Assert
            _mockUnitOfWork.Verify(u => u.Appointments.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
