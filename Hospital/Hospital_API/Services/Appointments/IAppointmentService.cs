using Hospital_API.Dto;

namespace Hospital_API.Services.Appointments
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task<bool> ExistsByPatientAndDoctorIdAsync(int patientId,int doctorId);
        Task<Appointment> GetAppointmentByPatientAndDoctorIdAsync(int patientId, int doctorId);
        Task CreateAppointmentAsync(AppointmentDTO appointmentDTO);
        Task DeleteAppointmentByPatientAndDoctorIdAsync(int patientId, int doctorId);
        Task EditAppointmentAsync(AppointmentDTO appointmentDTO);
    }
}
