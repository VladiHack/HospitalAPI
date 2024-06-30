using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/AppointmentAPI")]
    [ApiController]
    public class AppointmentAPIController : ControllerBase
    {
        public HospitalDbContext _context;

        public AppointmentAPIController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Appointment>> GetAppointments()
        {
            return Ok(_context.Appointments.ToList());
        }


        [HttpGet("{patientId:int}, {doctorId:int}", Name = "GetAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Appointment> GetAppointment(int patientId, int doctorId)
        {
            if (patientId < 0 || doctorId < 0)
            {
                return BadRequest();
            }
            var appointment = _context.Appointments.FirstOrDefault(u => u.DoctorId == doctorId && u.PatientId == patientId);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AppointmentDTO> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
            {
                return BadRequest(appointmentDTO);
            }

            // Check if an appointment with the same patient, doctor, and date already exists
            var existingAppointment = _context.Appointments.FirstOrDefault(a =>
                a.PatientId == appointmentDTO.PatientId &&
                a.DoctorId == appointmentDTO.DoctorId);

            if (existingAppointment != null)
            {
                ModelState.AddModelError("CustomError", "Appointment already exists!");
                return BadRequest(ModelState);
            }

            // Fetch the Patient and Doctor entities
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == appointmentDTO.PatientId);
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == appointmentDTO.DoctorId);

            if (patient == null)
            {
                ModelState.AddModelError("PatientId", "Invalid patient ID.");
                return BadRequest(ModelState);
            }

            if (doctor == null)
            {
                ModelState.AddModelError("DoctorId", "Invalid doctor ID.");
                return BadRequest(ModelState);
            }

            // Create a new Appointment entity from the AppointmentDTO
            var appointment = new Appointment
            {
                PatientId = appointmentDTO.PatientId,
                DoctorId = appointmentDTO.DoctorId,
                Date = DateOnly.Parse(appointmentDTO.Date),
                Patient = patient,
                Doctor = doctor
            };

            // Add the new appointment to the database
            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            // Return the created appointment as an AppointmentDTO
            return CreatedAtRoute("GetAppointment", new { patientId = appointmentDTO.PatientId, doctorId = appointmentDTO.DoctorId }, appointmentDTO);
        }

        [HttpDelete("{patientId},{doctorId}", Name = "DeleteAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteAppointment(int patientId, int doctorId)
        {
            if (patientId < 0 || doctorId < 0)
            {
                return BadRequest();
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.PatientId == patientId && a.DoctorId == doctorId);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();
            return NoContent();
        }

       

        [HttpPut("{patientId},{doctorId}", Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateAppointment(int patientId, int doctorId, [FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null || patientId != appointmentDTO.PatientId || doctorId != appointmentDTO.DoctorId)
            {
                return BadRequest();
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.PatientId == patientId && a.DoctorId == doctorId);
            if (appointment == null)
            {
                return NotFound();
            }

            // Fetch the Patient and Doctor entities
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == appointmentDTO.PatientId);
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == appointmentDTO.DoctorId);

            if (patient == null)
            {
                ModelState.AddModelError("PatientId", "Invalid patient ID.");
                return BadRequest(ModelState);
            }

            if (doctor == null)
            {
                ModelState.AddModelError("DoctorId", "Invalid doctor ID.");
                return BadRequest(ModelState);
            }

            appointment.Date = DateOnly.Parse(appointmentDTO.Date);
            appointment.Patient = patient;
            appointment.Doctor = doctor;

            _context.Appointments.Update(appointment);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
