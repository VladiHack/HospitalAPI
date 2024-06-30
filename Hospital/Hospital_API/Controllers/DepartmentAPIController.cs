using AutoMapper;
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/DepartmentAPI")]
    [ApiController]
    public class DepartmentAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentsAsync()
        {
            return Ok(await _context.Departments.ToListAsync());
        }


        [HttpGet("{id:int}", Name = "GetDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Department>> GetDepartmentAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var department = await _context.Departments.FirstOrDefaultAsync(u => u.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<Department>> CreateDepartmentAsync([FromBody] DepartmentDTO departmentDTO)
        {
            if (await _context.Departments.FirstOrDefaultAsync(u => u.DepartmentName.ToLower() == departmentDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Department already exists!");
                return BadRequest(ModelState);
            }
            if (departmentDTO == null)
            {
                return BadRequest(departmentDTO);
            }
            if (departmentDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var department = _mapper.Map<Department>(departmentDTO);

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetDoctor", new { id = departmentDTO.Id }, departmentDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteDepartment")]

        public async Task<ActionResult> DeleteDepartmentAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var department = await _context.Departments.FirstOrDefaultAsync(u => u.DepartmentId == id);
            if (department == null) return NotFound();

            //Delete all doctors,staff, and appointments connected to the department
            List<Doctor> doctors = await _context.Doctors.Where(u => u.DepartmentId == id).ToListAsync();
            foreach (Doctor doctor in doctors)
            {
                //Remove all appointments for each doctor
                List<Appointment> appointments = await _context.Appointments.Where(a => a.DoctorId == doctor.DoctorId).ToListAsync();
                foreach (Appointment appointment in appointments)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();
                }
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
            List<Staff> staff = await _context.Staff.Where(u => u.DepartmentId == id).ToListAsync();
            foreach (Staff staffMember in staff)
            {
                _context.Staff.Remove(staffMember);
                await _context.SaveChangesAsync();
            }


            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdateDepartmentAsync(int id, [FromBody] DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null || id != departmentDTO.Id)
            {
                return BadRequest();
            }

            var department = _mapper.Map<Department>(departmentDTO);
          
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

