
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/DepartmentAPI")]
    [ApiController]
    public class DepartmentAPIController: ControllerBase
    {
        public HospitalDbContext _context;

        public DepartmentAPIController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Department>> GetDepartments()
        {
            return Ok(_context.Departments.ToList());
        }


        [HttpGet("{id:int}", Name = "GetDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Department> GetDepartment(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var department = _context.Departments.FirstOrDefault(u => u.DepartmentId == id);
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

        public ActionResult<Department> CreateDepartment([FromBody] DepartmentDTO departmentDTO)
        {
            if (_context.Departments.FirstOrDefault(u => u.DepartmentName.ToLower() == departmentDTO.Name.ToLower()) != null)
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

            Department department = new Department();

            department.DepartmentName = departmentDTO.Name;
            department.HospitalId = departmentDTO.HospitalId;

            _context.Departments.Add(department);
            _context.SaveChanges();
            return CreatedAtRoute("GetDoctor", new { id = departmentDTO.Id }, departmentDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteDepartment")]

        public ActionResult DeleteDepartment(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var department = _context.Departments.FirstOrDefault(u => u.DepartmentId == id);
            if (department == null) return NotFound();

            //Delete all doctors,staff, and appointments connected to the department
            List<Doctor> doctors = _context.Doctors.Where(u => u.DepartmentId == id).ToList();
            foreach (Doctor doctor in doctors)
            {
               //Remove all appointments for each doctor
                List<Appointment> appointments = _context.Appointments.Where(a => a.DoctorId == doctor.DoctorId).ToList();
                foreach(Appointment appointment  in appointments)
                {
                    _context.Appointments.Remove(appointment);
                    _context.SaveChanges();
                }
                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
            }
            List<Staff> staff=_context.Staff.Where(u=>u.DepartmentId == id).ToList();
            foreach(Staff staffMember in staff)
            {
                _context.Staff.Remove(staffMember);
                _context.SaveChanges();
            }
         

            _context.Departments.Remove(department);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateDepartment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateDepartment(int id, [FromBody] DepartmentDTO departmentDTO)
        {
            if (departmentDTO == null || id != departmentDTO.Id)
            {
                return BadRequest();
            }

            Department department = _context.Departments.FirstOrDefault(u => u.DepartmentId == id);
            department.DepartmentName = departmentDTO.Name;
            department.HospitalId = departmentDTO.HospitalId;

            _context.Departments.Update(department);
            _context.SaveChanges();
            return NoContent();
        }

      
    }
}
