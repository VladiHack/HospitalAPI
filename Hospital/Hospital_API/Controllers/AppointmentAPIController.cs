using AutoMapper;
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/AppointmentAPI")]
    [ApiController]
    public class AppointmentAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public AppointmentAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsAsync()
        {
            return Ok(await _context.Appointments.ToListAsync());
        }

        [HttpGet("{patientId:int}, {doctorId:int}", Name = "GetAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Appointment>> GetAppointmentAsync(int patientId, int doctorId)
        {
            if (patientId < 0 || doctorId < 0)
            {
                return BadRequest();
            }
            var appointment = await _context.Appointments.FirstOrDefaultAsync(u => u.DoctorId == doctorId && u.PatientId == patientId);
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
        public async Task<ActionResult<AppointmentDTO>> CreateAppointmentAsync([FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
            {
                return BadRequest(appointmentDTO);
            }

            // Check if an appointment with the same patient, doctor, and date already exists
            var existingAppointment = await _context.Appointments.FirstOrDefaultAsync(a =>
                a.PatientId == appointmentDTO.PatientId &&
                a.DoctorId == appointmentDTO.DoctorId);

            if (existingAppointment != null)
            {
                ModelState.AddModelError("CustomError", "Appointment already exists!");
                return BadRequest(ModelState);
            }

            // Fetch the Patient and Doctor entities
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == appointmentDTO.PatientId);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == appointmentDTO.DoctorId);

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
            var appointment = _mapper.Map<Appointment>(appointmentDTO);
        
            // Add the new appointment to the database
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            // Return the created appointment as an AppointmentDTO
            return CreatedAtRoute("GetAppointment", new { patientId = appointmentDTO.PatientId, doctorId = appointmentDTO.DoctorId }, appointmentDTO);
        }

        [HttpDelete("{patientId},{doctorId}", Name = "DeleteAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAppointmentAsync(int patientId, int doctorId)
        {
            if (patientId < 0 || doctorId < 0)
            {
                return BadRequest();
            }

            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.PatientId == patientId && a.DoctorId == doctorId);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{patientId},{doctorId}", Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAppointmentAsync(int patientId, int doctorId, [FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null || patientId != appointmentDTO.PatientId || doctorId != appointmentDTO.DoctorId)
            {
                return BadRequest();
            }

            var appointment = _mapper.Map<Appointment>(appointmentDTO);
            if (appointment == null)
            {
                return NotFound();
            }

            // Fetch the Patient and Doctor entities
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == appointmentDTO.PatientId);
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == appointmentDTO.DoctorId);

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

            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}