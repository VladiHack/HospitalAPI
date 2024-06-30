using AutoMapper;
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/DoctorAPI")]
    [ApiController]
    public class DoctorAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public DoctorAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsAsync()
        {
            return Ok(await _context.Doctors.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Doctor>> GetDoctorAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(u => u.DoctorId == id);
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
        public async Task<ActionResult<Doctor>> CreateDoctorAsync([FromBody] DoctorDTO doctorDTO)
        {
            if (await _context.Doctors.FirstOrDefaultAsync(u => u.DoctorFirstName.ToLower() == doctorDTO.FirstName.ToLower() && u.DoctorLastName.ToLower() == doctorDTO.LastName.ToLower()) != null)
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

            var doctor = _mapper.Map<Doctor>(doctorDTO);

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetDoctor", new { id = doctorDTO.Id }, doctorDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDoctorAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var doctor = await _context.Doctors.FirstOrDefaultAsync(u => u.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            // Delete all appointments of the doctor
            List<Appointment> appointments = await _context.Appointments.Where(u => u.DoctorId == id).ToListAsync();
            foreach (Appointment appointment in appointments)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDoctorAsync(int id, [FromBody] DoctorDTO doctorDTO)
        {
            if (doctorDTO == null || id != doctorDTO.Id)
            {
                return BadRequest();
            }

            var doctor = _mapper.Map<Doctor>(doctorDTO);

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}