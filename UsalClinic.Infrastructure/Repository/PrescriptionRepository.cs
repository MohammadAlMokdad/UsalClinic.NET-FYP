using UsalClinic.Core.Entities;
using UsalClinic.Web.Data;
using Microsoft.EntityFrameworkCore;


public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prescription>> GetAllAsync()
    {
        return await _context.Prescriptions.Include(p => p.MedicalRecord).ToListAsync();
    }

    public async Task<Prescription> GetByIdAsync(int id)
    {
        return await _context.Prescriptions
       .Include(p => p.MedicalRecord) 
       .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Prescription prescription)
    {
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Prescription prescription)
    {
        _context.Prescriptions.Update(prescription);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Prescriptions.FindAsync(id);
        if (entity != null)
        {
            _context.Prescriptions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<Prescription>> GetPrescriptionsByMedicalRecordIdAsync(int medicalId)
    {
        return await _context.Prescriptions
        .Include(p => p.MedicalRecord) 
        .Where(mr => mr.MedicalRecordId == medicalId)
        .ToListAsync();
    }
}
