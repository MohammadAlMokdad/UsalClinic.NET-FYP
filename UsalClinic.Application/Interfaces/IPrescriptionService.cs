using UsalClinic.Application.Models;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDto>> GetAllAsync();
    Task<PrescriptionDto> GetByIdAsync(int id);
    Task CreateAsync(PrescriptionDto dto);
    Task UpdateAsync(PrescriptionDto dto);
    Task DeleteAsync(int id);
}
