using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core;
using UsalClinic.Core.Entities;
using Xunit;
using Microsoft.Extensions.Logging;  // for ILogger<T>


namespace UsalClinic.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new DepartmentService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllDepartmentsAsync_ReturnsMappedDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "Cardiology" },
                new Department { Id = 2, Name = "Neurology" }
            };
            var departmentDtos = new List<DepartmentDto>
            {
                new DepartmentDto { Id = 1, Name = "Cardiology" },
                new DepartmentDto { Id = 2, Name = "Neurology" }
            };

            _mockUnitOfWork.Setup(u => u.Departments.GetAllAsync()).ReturnsAsync(departments);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(departments)).Returns(departmentDtos);

            // Act
            var result = await _service.GetAllDepartmentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_ExistingId_ReturnsMappedDto()
        {
            // Arrange
            var department = new Department { Id = 1, Name = "Cardiology" };
            var departmentDto = new DepartmentDto { Id = 1, Name = "Cardiology" };

            _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(department)).Returns(departmentDto);

            // Act
            var result = await _service.GetDepartmentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(99)).ReturnsAsync((Department?)null);

            // Act
            var result = await _service.GetDepartmentByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddDepartmentAsync_ValidDto_CallsAddAndSave()
        {
            // Arrange
            var dto = new DepartmentDto { Id = 1, Name = "Oncology" };
            var entity = new Department { Id = 1, Name = "Oncology" };

            _mockMapper.Setup(m => m.Map<Department>(dto)).Returns(entity);

            // Act
            await _service.AddDepartmentAsync(dto);

            // Assert
            _mockUnitOfWork.Verify(u => u.Departments.AddAsync(entity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_ExistingDepartment_UpdatesAndSaves()
        {
            // Arrange
            var dto = new DepartmentDto { Id = 1, Name = "Surgery" };
            var existing = new Department { Id = 1, Name = "Old Surgery" };

            _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(1)).ReturnsAsync(existing);

            // Act
            await _service.UpdateDepartmentAsync(dto);

            // Assert
            _mockMapper.Verify(m => m.Map(dto, existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.Departments.UpdateAsync(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_NonExistingDepartment_DoesNothing()
        {
            // Arrange
            var dto = new DepartmentDto { Id = 99, Name = "Unknown" };
            _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(99)).ReturnsAsync((Department?)null);

            // Act
            await _service.UpdateDepartmentAsync(dto);

            // Assert
            _mockMapper.Verify(m => m.Map(It.IsAny<DepartmentDto>(), It.IsAny<Department>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.Departments.UpdateAsync(It.IsAny<Department>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteDepartmentAsync_CallsDeleteAndSave()
        {
            // Act
            await _service.DeleteDepartmentAsync(1);

            // Assert
            _mockUnitOfWork.Verify(u => u.Departments.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
