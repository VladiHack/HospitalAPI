using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/DoctorAPI")]
    [ApiController]
    public class DoctorAPIController:ControllerBase
    {
        public HospitalDbContext _context;

        public DoctorAPIController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Doctor>> GetDoctors()
        {
            return Ok(_context.Doctors.ToList());
        }


        [HttpGet("{id:int}", Name = "GetDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Doctor> GetDoctor(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var doctor = _context.Doctors.FirstOrDefault(u => u.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<Doctor> CreateDoctor([FromBody] DoctorDTO doctorDTO)
        {
            if (_context.Doctors.FirstOrDefault(u => u.DoctorFirstName.ToLower() == doctorDTO.FirstName.ToLower() && u.DoctorLastName.ToLower()==doctorDTO.LastName.ToLower())!= null)
            {
                ModelState.AddModelError("CustomError", "Doctor already exists!");
                return BadRequest(ModelState);
            }
            if (doctorDTO == null)
            {
                return BadRequest(doctorDTO);
            }
            if (doctorDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Doctor doctor = new Doctor();

            doctor.DoctorFirstName = doctorDTO.FirstName;
            doctor.DoctorLastName = doctorDTO.LastName;
            doctor.DepartmentId = doctorDTO.DepartmentId;
            doctor.DoctorPhoneNumber=doctorDTO.PhoneNumber;

            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            return CreatedAtRoute("GetDoctor", new { id = doctorDTO.Id }, doctorDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteDoctor")]

        public ActionResult DeleteDoctor(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var doctor = _context.Doctors.FirstOrDefault(u => u.DoctorId == id);
            if (doctor == null) return NotFound();

            //Delete all appointments of the doctor
            List<Appointment> appointments=_context.Appointments.Where(u=>u.DoctorId==id).ToList();
            foreach (Appointment appointment in appointments)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }

            _context.Doctors.Remove(doctor);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateDoctor(int id, [FromBody] DoctorDTO doctorDTO)
        {
            if (doctorDTO == null || id != doctorDTO.Id)
            {
                return BadRequest();
            }

            Doctor doctor = _context.Doctors.FirstOrDefault(u => u.DoctorId == id);

            doctor.DoctorFirstName = doctorDTO.FirstName;
            doctor.DoctorLastName = doctorDTO.LastName;
            doctor.DepartmentId = doctorDTO.DepartmentId;
            doctor.DoctorPhoneNumber = doctorDTO.PhoneNumber;

            _context.Doctors.Update(doctor);
            _context.SaveChanges();
            return NoContent();
        }

        
    }
}

