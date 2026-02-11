using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsalClinic.Application.Models; 

namespace UsalClinic.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto departmentDto);
        Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId);
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto> UpdateDepartmentAsync(DepartmentDto departmentDto);
        Task<bool> DeleteDepartmentAsync(int departmentId);
        Task<IEnumerable<DepartmentDto>> GetDepartmentsByDoctorAsync(Guid doctorId);
        Task<IEnumerable<DepartmentDto>> GetDepartmentsByRoomAsync(int roomId);
    }
}
