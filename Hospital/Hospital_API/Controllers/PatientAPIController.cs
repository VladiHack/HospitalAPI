using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/PatientAPI")]
    [ApiController]
    public class PatientAPIController:ControllerBase
    {
        private readonly HospitalDbContext _context;
        public PatientAPIController(HospitalDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            return Ok(_context.Patients.ToList());
        }

        [HttpGet("{id:int}", Name = "GetPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Patient> GetPatient(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var patient = _context.Patients.FirstOrDefault(u => u.PatientId == id);
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

        public ActionResult<Patient> CreatePatient([FromBody] PatientDTO patientDTO)
        {
            if (_context.Patients.FirstOrDefault(u => u.PatientFirstName.ToLower() == patientDTO.FirstName.ToLower() && u.PatientLastName.ToLower() == patientDTO.LastName.ToLower()) != null)
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

            Patient patient = new Patient();

            patient.PatientFirstName = patientDTO.FirstName;
            patient.PatientLastName = patientDTO.LastName;
            patient.PatientAddress = patientDTO.Address;
            patient.PatientPhoneNumber = patientDTO.PhoneNumber;

            _context.Patients.Add(patient);
            _context.SaveChanges();
            return CreatedAtRoute("GetPatient", new { id = patientDTO.Id }, patientDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeletePatient")]

        public ActionResult DeletePatient(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var patient = _context.Patients.FirstOrDefault(u => u.PatientId == id);
            if (patient == null) return NotFound();

            //Delete all appointments of the patient
            List<Appointment> appointments = _context.Appointments.Where(u=>u.PatientId == id).ToList();
            foreach (Appointment appointment in appointments)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }

            _context.Patients.Remove(patient);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePatient(int id, [FromBody] PatientDTO patientDTO)
        {
            if (patientDTO == null || id != patientDTO.Id)
            {
                return BadRequest();
            }

            Patient patient = _context.Patients.FirstOrDefault(u => u.PatientId == id);

            patient.PatientFirstName = patientDTO.FirstName;
            patient.PatientLastName = patientDTO.LastName;
            patient.PatientAddress = patientDTO.Address;
            patient.PatientPhoneNumber = patientDTO.PhoneNumber;

            _context.Patients.Update(patient);
            _context.SaveChanges();
            return NoContent();
        }

               

    }
}

