using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core;
using UsalClinic.Core.Entities;
using Xunit;

namespace UsalClinic.Tests.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DoctorService>> _mockLogger;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _mockLogger = new Mock<ILogger<DoctorService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new DoctorService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllDoctorsAsync_ReturnsMappedDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = Guid.NewGuid(),
                    UserId = "user1",
                    User = new ApplicationUser { FullName = "Dr. Ahmad" },
                    Profession = "Cardiology",
                    YearsOfExperience = 10,
                    Address = "123 Street",
                    Gender = "M",
                    DateOfBirth = new DateTime(1980, 1, 1)
                },
                new Doctor
                {
                    Id = Guid.NewGuid(),
                    UserId = "user2",
                    User = new ApplicationUser { FullName = "Dr. Hadi" },
                    Profession = "Neurology",
                    YearsOfExperience = 8,
                    Address = "456 Avenue",
                    Gender = "M",
                    DateOfBirth = new DateTime(1985, 5, 15)
                }
            };

            var doctorDtos = doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Profession = d.Profession,
                YearsOfExperience = d.YearsOfExperience,
                Address = d.Address,
                Gender = d.Gender,
                DateOfBirth = d.DateOfBirth
            }).ToList();

            _mockUnitOfWork.Setup(u => u.Doctors.GetAllDoctorsAsync()).ReturnsAsync(doctors);
            _mockMapper.Setup(m => m.Map<List<DoctorDto>>(doctors)).Returns(doctorDtos);

            // Act
            var result = await _service.GetAllDoctorsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Dr. Ahmad", result[0].FullName);
            Assert.Equal("Dr. Hadi", result[1].FullName);
        }

        [Fact]
        public async Task GetDoctorByIdAsync_ExistingDoctor_ReturnsMappedDto()
        {
            var id = Guid.NewGuid();
            var doctor = new Doctor
            {
                Id = id,
                UserId = "user123",
                User = new ApplicationUser { FullName = "Dr. Ahmad" },
                Profession = "Cardiology",
                YearsOfExperience = 10,
                Address = "123 Clinic St",
                Gender = "M",
                DateOfBirth = new DateTime(1980, 1, 1)
            };

            var dto = new DoctorDto
            {
                Id = id,
                FullName = "Dr. Ahmad",
                Profession = doctor.Profession,
                YearsOfExperience = doctor.YearsOfExperience,
                Address = doctor.Address,
                Gender = doctor.Gender,
                DateOfBirth = doctor.DateOfBirth
            };

            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(id)).ReturnsAsync(doctor);
            _mockMapper.Setup(m => m.Map<DoctorDto>(doctor)).Returns(dto);

            var result = await _service.GetDoctorByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Dr. Ahmad", result?.FullName);
        }

        [Fact]
        public async Task GetDoctorByIdAsync_NonExistingDoctor_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((Doctor?)null);

            var result = await _service.GetDoctorByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDoctorAsync_ValidDto_CreatesAndReturnsDoctor()
        {
            var dto = new DoctorDto { FullName = "New Doctor" };
            var entity = new Doctor
            {
                User = new ApplicationUser { FullName = "New Doctor" }
            };
            var returnDto = new DoctorDto { Id = Guid.NewGuid(), FullName = "New Doctor" };

            _mockMapper.Setup(m => m.Map<Doctor>(dto)).Returns(entity);
            _mockMapper.Setup(m => m.Map<DoctorDto>(It.IsAny<Doctor>())).Returns(returnDto);

            var result = await _service.CreateDoctorAsync(dto);

            _mockUnitOfWork.Verify(u => u.Doctors.AddAsync(It.IsAny<Doctor>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            Assert.Equal("New Doctor", result.FullName);
        }

        [Fact]
        public async Task UpdateDoctorGuidAsync_ExistingDoctor_UpdatesAndReturnsDto()
        {
            var id = Guid.NewGuid();
            var existing = new Doctor
            {
                Id = id,
                User = new ApplicationUser { FullName = "Old Name" }
            };
            var updatedDto = new DoctorDto { Id = id, FullName = "New Name" };

            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(id)).ReturnsAsync(existing);
            _mockMapper.Setup(m => m.Map(updatedDto, existing));
            _mockMapper.Setup(m => m.Map<DoctorDto>(existing)).Returns(updatedDto);

            var result = await _service.UpdateDoctorGuidAsync(id, updatedDto);

            Assert.NotNull(result);
            Assert.Equal("New Name", result?.FullName);
        }

        [Fact]
        public async Task UpdateDoctorGuidAsync_NonExistingDoctor_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((Doctor?)null);

            var result = await _service.UpdateDoctorGuidAsync(Guid.NewGuid(), new DoctorDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateDoctorAsync_ExistingDoctor_UpdatesAndReturnsDto()
        {
            var id = Guid.NewGuid();
            var dto = new DoctorDto { Id = id, FullName = "Updated" };
            var entity = new Doctor { Id = id, User = new ApplicationUser { FullName = "Old" } };

            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(id)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map(dto, entity));
            _mockMapper.Setup(m => m.Map<DoctorDto>(entity)).Returns(dto);

            var result = await _service.UpdateDoctorAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.FullName);
        }

        [Fact]
        public async Task UpdateDoctorAsync_NonExistingDoctor_ThrowsException()
        {
            var dto = new DoctorDto { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(dto.Id))
                           .ReturnsAsync((Doctor?)null);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.UpdateDoctorAsync(dto));
            Assert.Contains("does not exist", ex.Message);
        }

        [Fact]
        public async Task DeleteDoctorAsync_ExistingDoctor_ReturnsTrue()
        {
            var id = Guid.NewGuid();
            var doctor = new Doctor { Id = id, User = new ApplicationUser { FullName = "Delete Doctor" } };

            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(id)).ReturnsAsync(doctor);

            var result = await _service.DeleteDoctorAsync(id);

            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Doctors.DeleteAsync(doctor), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteDoctorAsync_NonExistingDoctor_ReturnsFalse()
        {
            _mockUnitOfWork.Setup(u => u.Doctors.GetDoctorByIdAsync(It.IsAny<Guid>()))
                           .ReturnsAsync((Doctor?)null);

            var result = await _service.DeleteDoctorAsync(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task GetDoctorByUserIdAsync_ExistingDoctor_ReturnsMappedDto()
        {
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                UserId = "user123",
                User = new ApplicationUser { FullName = "Dr. UserId" }
            };

            var dto = new DoctorDto
            {
                Id = doctor.Id,
                UserId = doctor.UserId,
                FullName = doctor.User.FullName
            };

            _mockUnitOfWork.Setup(u => u.Doctors.GetByUserIdAsync("user123")).ReturnsAsync(doctor);
            _mockMapper.Setup(m => m.Map<DoctorDto>(doctor)).Returns(dto);

            var result = await _service.GetDoctorByUserIdAsync("user123");

            Assert.NotNull(result);
            Assert.Equal("Dr. UserId", result?.FullName);
        }

        [Fact]
        public async Task GetDoctorByUserIdAsync_NonExisting_ReturnsNull()
        {
            _mockUnitOfWork.Setup(u => u.Doctors.GetByUserIdAsync("invalid"))
                           .ReturnsAsync((Doctor?)null);

            var result = await _service.GetDoctorByUserIdAsync("invalid");

            Assert.Null(result);
        }
    }
}
