
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Services.Doctors
{
    public class DoctorService : IDoctorService
    {
        private readonly HospitalDbContext _context;

        public DoctorService(HospitalDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByIdAsync(int id) => await _context.Doctors.AnyAsync(d => d.DoctorId == id);
    }
}
