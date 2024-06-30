using Hospital_API.Dto;
using Hospital_API.Services.Appointments;
using Hospital_API.Services.Doctors;
using Hospital_API.Services.Patients;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/AppointmentAPI")]
    [ApiController]
    public class AppointmentAPIController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public AppointmentAPIController(IAppointmentService appointmentService, IDoctorService doctorService, IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsAsync()
        {
            return Ok(await _appointmentService.GetAppointmentsAsync());
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

            if (!await _appointmentService.ExistsByPatientAndDoctorIdAsync(patientId, doctorId))
            {
                return NotFound();
            }

            var appointment = await _appointmentService.GetAppointmentByPatientAndDoctorIdAsync(patientId, doctorId);
           
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

            if (await _appointmentService.ExistsByPatientAndDoctorIdAsync(appointmentDTO.PatientId,appointmentDTO.DoctorId))
            {
                ModelState.AddModelError("CustomError", "Appointment already exists!");
                return BadRequest(ModelState);
            }

            if (!await _patientService.ExistsByIdAsync(appointmentDTO.PatientId))
            {
                ModelState.AddModelError("PatientId", "Invalid patient ID.");
                return BadRequest(ModelState);
            }

            if (!await _doctorService.ExistsByIdAsync(appointmentDTO.DoctorId))
            {
                ModelState.AddModelError("DoctorId", "Invalid doctor ID.");
                return BadRequest(ModelState);
            }

            await _appointmentService.CreateAppointmentAsync(appointmentDTO);

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

            if (!await _appointmentService.ExistsByPatientAndDoctorIdAsync(patientId, doctorId))
            {
                return NotFound();
            }

            await _appointmentService.DeleteAppointmentByPatientAndDoctorIdAsync(patientId, doctorId);
            return Ok("Successfully deleted appointment.");
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

            if (!await _appointmentService.ExistsByPatientAndDoctorIdAsync(patientId, doctorId))
            {
                return NotFound();
            }

            if (!await _patientService.ExistsByIdAsync(appointmentDTO.PatientId))
            {
                ModelState.AddModelError("PatientId", "Invalid patient ID.");
                return BadRequest(ModelState);
            }

            if (!await _doctorService.ExistsByIdAsync(appointmentDTO.DoctorId))
            {
                ModelState.AddModelError("DoctorId", "Invalid doctor ID.");
                return BadRequest(ModelState);
            }

            await _appointmentService.EditAppointmentAsync(appointmentDTO);
            return Ok("Successfully edited appointment.");
        }
    }
}