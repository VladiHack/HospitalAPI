using AutoMapper;
using Hospital_API.Dto;
using Hospital_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/HospitalAPI")]
    [ApiController]
    public class HospitalAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public HospitalAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitalsAsync()
        {
            return Ok(await _context.Hospitals.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetHospital")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Hospital>> GetHospitalAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var hospital = await _context.Hospitals.FirstOrDefaultAsync(u => u.HospitalId == id);
            if (hospital == null)
            {
                return NotFound();
            }
            return Ok(hospital);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Hospital>> CreateHospitalAsync([FromBody] HospitalDTO hospitalDTO)
        {
            if (await _context.Hospitals.AnyAsync(u => u.HospitalName.ToLower() == hospitalDTO.Name.ToLower()))
            {
                ModelState.AddModelError("CustomError", "Hospital already exists!");
                return BadRequest(ModelState);
            }
            if (hospitalDTO == null)
            {
                return BadRequest(hospitalDTO);
            }
            if (hospitalDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var hospital = _mapper.Map<Hospital>(hospitalDTO);
         
            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetHospital", new { id = hospitalDTO.Id }, hospitalDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteHospital")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHospitalAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var hospital = await _context.Hospitals.FirstOrDefaultAsync(u => u.HospitalId == id);
            if (hospital == null)
            {
                return NotFound();
            }

            // Remove all departments which are linked to the hospital
            var departments = await _context.Departments.Where(u => u.HospitalId == id).ToListAsync();
            foreach (var department in departments)
            {
                // Delete all doctors and staff in this department
                var doctors = await _context.Doctors.Where(u => u.DepartmentId == department.DepartmentId).ToListAsync();
                foreach (var doctor in doctors)
                {
                    // Delete all appointments of the doctor
                    var appointments = await _context.Appointments.Where(a => a.DoctorId == doctor.DoctorId).ToListAsync();
                    foreach (var appointment in appointments)
                    {
                        _context.Appointments.Remove(appointment);
                        await _context.SaveChangesAsync();
                    }

                    _context.Doctors.Remove(doctor);
                    await _context.SaveChangesAsync();
                }

                var staff = await _context.Staff.Where(u => u.DepartmentId == department.DepartmentId).ToListAsync();
                foreach (var staffMember in staff)
                {
                    _context.Staff.Remove(staffMember);
                    await _context.SaveChangesAsync();
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateHospital")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateHospitalAsync(int id, [FromBody] HospitalDTO hospitalDTO)
        {
            if (hospitalDTO == null || id != hospitalDTO.Id)
            {
                return BadRequest();
            }

            var hospital = _mapper.Map<Hospital>(hospitalDTO);
            if (hospital == null)
            {
                return NotFound();
            }

            _context.Hospitals.Update(hospital);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}