using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/StaffAPI")]
    [ApiController]
    public class StaffAPIController:ControllerBase
    {

        private readonly HospitalDbContext _context;
        public StaffAPIController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Staff>> GetStaffMembers()
        {
            return Ok(_context.Staff.ToList());
        }


        [HttpGet("{id:int}", Name = "GetStaffMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Staff> GetStaffMember(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var staffMember = _context.Staff.FirstOrDefault(u => u.StaffId == id);
            if (staffMember == null)
            {
                return NotFound();
            }
            return Ok(staffMember);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<Staff> CreateStaff([FromBody] StaffDTO staffDTO)
        {
            if (_context.Staff.FirstOrDefault(u => u.StaffFirstName.ToLower() == staffDTO.FirstName.ToLower() && u.StaffLastName.ToLower() == staffDTO.LastName.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Staff member already exists!");
                return BadRequest(ModelState);
            }
            if (staffDTO == null)
            {
                return BadRequest(staffDTO);
            }
            if (staffDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Staff staffMember = new Staff();

            staffMember.StaffFirstName = staffDTO.FirstName;
            staffMember.StaffLastName = staffDTO.LastName;
            staffMember.DepartmentId = staffDTO.DepartmentId;
            staffMember.StaffPhoneNumber = staffDTO.PhoneNumber;
            staffMember.StaffAddress = staffDTO.Address;

            _context.Staff.Add(staffMember);
            _context.SaveChanges();
            return CreatedAtRoute("GetStaffMember", new { id = staffDTO.Id }, staffDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteStaffMember")]

        public ActionResult DeleteStaffMember(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var staffMember = _context.Staff.FirstOrDefault(u => u.StaffId == id);
            if (staffMember == null) return NotFound();

            _context.Staff.Remove(staffMember);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateStaffMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateStaffMember(int id, [FromBody] StaffDTO staffDTO)
        {
            if (staffDTO == null || id != staffDTO.Id)
            {
                return BadRequest();
            }

            Staff staffMember = _context.Staff.FirstOrDefault(u => u.StaffId == id);

            staffMember.StaffFirstName = staffDTO.FirstName;
            staffMember.StaffLastName = staffDTO.LastName;
            staffMember.DepartmentId = staffDTO.DepartmentId;
            staffMember.StaffPhoneNumber = staffDTO.PhoneNumber;
            staffMember.StaffAddress = staffDTO.Address;

            _context.Staff.Update(staffMember);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
