
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Services.Patients
{
    public class PatientService : IPatientService
    {
        private readonly HospitalDbContext _context;

        public PatientService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByIdAsync(int id) => await _context.Patients.AnyAsync(p => p.PatientId == id);
    }
}
