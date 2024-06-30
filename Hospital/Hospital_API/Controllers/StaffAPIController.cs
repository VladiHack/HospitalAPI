using AutoMapper;
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_API.Controllers
{
    [Route("api/StaffAPI")]
    [ApiController]
    public class StaffAPIController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public StaffAPIController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffMembersAsync()
        {
            return Ok(await _context.Staff.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetStaffMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Staff>> GetStaffMemberAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var staffMember = await _context.Staff.FirstOrDefaultAsync(u => u.StaffId == id);
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
        public async Task<ActionResult<Staff>> CreateStaffAsync([FromBody] StaffDTO staffDTO)
        {
            if (await _context.Staff.FirstOrDefaultAsync(u => u.StaffFirstName.ToLower() == staffDTO.FirstName.ToLower() && u.StaffLastName.ToLower() == staffDTO.LastName.ToLower()) != null)
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

            var staffMember = _mapper.Map<Staff>(staffDTO);

            _context.Staff.Add(staffMember);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetStaffMember", new { id = staffDTO.Id }, staffDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteStaffMember")]
        public async Task<ActionResult> DeleteStaffMemberAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var staffMember = await _context.Staff.FirstOrDefaultAsync(u => u.StaffId == id);
            if (staffMember == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staffMember);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateStaffMember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStaffMemberAsync(int id, [FromBody] StaffDTO staffDTO)
        {
            if (staffDTO == null || id != staffDTO.Id)
            {
                return BadRequest();
            }

            var staffMember = _mapper.Map<Staff>(staffDTO);
            if (staffMember == null)
            {
                return NotFound();
            }

            _context.Staff.Update(staffMember);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}