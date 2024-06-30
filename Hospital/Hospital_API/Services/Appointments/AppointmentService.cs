using AutoMapper;
using Hospital_API.Dto;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public AppointmentService(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ExistsByPatientAndDoctorIdAsync(int patientId, int doctorId) => await _context.Appointments.AnyAsync(a => a.PatientId == patientId && a.DoctorId == doctorId);

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync() => await _context.Appointments.ToListAsync();

        public async Task<Appointment> GetAppointmentByPatientAndDoctorIdAsync(int patientId, int doctorId) => await _context.Appointments.FirstAsync(a => a.PatientId == patientId && a.DoctorId == doctorId);

        public async Task CreateAppointmentAsync(AppointmentDTO appointmentDTO)
        {
            var appointment = _mapper.Map<Appointment>(appointmentDTO);

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentByPatientAndDoctorIdAsync(int patientId, int doctorId)
        {
            var appointmentToDelete = await GetAppointmentByPatientAndDoctorIdAsync(patientId, doctorId);

            _context.Appointments.Remove(appointmentToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task EditAppointmentAsync(AppointmentDTO appointmentDTO)
        {
            var appointment = _mapper.Map<Appointment>(appointmentDTO);

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
