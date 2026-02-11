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
    public class DepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DepartmentService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            _logger.LogInformation("Retrieving all departments.");
            var departments = await _unitOfWork.Departments.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving department with ID {DepartmentId}.", id);
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found.", id);
                return null;
            }

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task AddDepartmentAsync(DepartmentDto dto)
        {
            _logger.LogInformation("Adding a new department with Name: {Name}.", dto.Name);
            try
            {
                var department = _mapper.Map<Department>(dto);
                await _unitOfWork.Departments.AddAsync(department);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Department added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a department.");
                throw;
            }
        }

        public async Task UpdateDepartmentAsync(DepartmentDto dto)
        {
            _logger.LogInformation("Updating department with ID {DepartmentId}.", dto.Id);
            var existing = await _unitOfWork.Departments.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                _logger.LogWarning("Department with ID {DepartmentId} not found for update.", dto.Id);
                return;
            }

            _mapper.Map(dto, existing);
            await _unitOfWork.Departments.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Department with ID {DepartmentId} updated successfully.", dto.Id);
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            _logger.LogInformation("Deleting department with ID {DepartmentId}.", id);
            try
            {
                await _unitOfWork.Departments.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Department with ID {DepartmentId} deleted successfully.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting department with ID {DepartmentId}.", id);
                throw;
            }
        }
    }
}
