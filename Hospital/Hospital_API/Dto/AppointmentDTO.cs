using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Dto
{
    public class AppointmentDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Date { get; set; }
    }
}
