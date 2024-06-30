using AutoMapper;
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/PatientAPI")]
    [ApiController]
    public class PatientAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public PatientAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientsAsync()
        {
            return Ok(await _context.Patients.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> GetPatientAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var patient = await _context.Patients.FirstOrDefaultAsync(u => u.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Patient>> CreatePatientAsync([FromBody] PatientDTO patientDTO)
        {
            if (await _context.Patients.FirstOrDefaultAsync(u => u.PatientFirstName.ToLower() == patientDTO.FirstName.ToLower() && u.PatientLastName.ToLower() == patientDTO.LastName.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Patient already exists!");
                return BadRequest(ModelState);
            }
            if (patientDTO == null)
            {
                return BadRequest(patientDTO);
            }
            if (patientDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var patient = _mapper.Map<Patient>(patientDTO);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetPatient", new { id = patientDTO.Id }, patientDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeletePatient")]
        public async Task<ActionResult> DeletePatientAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var patient = await _context.Patients.FirstOrDefaultAsync(u => u.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            // Delete all appointments of the patient
            List<Appointment> appointments = await _context.Appointments.Where(u => u.PatientId == id).ToListAsync();
            foreach (Appointment appointment in appointments)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatientAsync(int id, [FromBody] PatientDTO patientDTO)
        {
            if (patientDTO == null || id != patientDTO.Id)
            {
                return BadRequest();
            }

            var patient = _mapper.Map<Patient>(patientDTO);

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}