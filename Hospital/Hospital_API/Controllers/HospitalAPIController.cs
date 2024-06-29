
using Hospital_API.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_API.Controllers
{
    [Route("api/Hospital")]
    [ApiController]
    public class HospitalAPIController:ControllerBase
    {
        public HospitalDbContext _context;

        public HospitalAPIController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Hospital>> GetHospitals()
        { 
            return Ok(_context.Hospitals.ToList()); 
        }


        [HttpGet("{id:int}",Name="GetHospital")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Hospital> GetHospital(int id)
        {
            if(id<0)
            {
                return BadRequest();
            }
            var hospital = _context.Hospitals.FirstOrDefault(u => u.HospitalId == id);
            if(hospital==null)
            {
                return NotFound();
            }
            return Ok(hospital);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<Hospital> CreateHospital([FromBody] HospitalDTO hospitalDTO)
        {
            if (_context.Hospitals.FirstOrDefault(u => u.HospitalName.ToLower() == hospitalDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "Hospital already exists!");
                return BadRequest(ModelState);
            }
            if(hospitalDTO==null)
            {
                return BadRequest(hospitalDTO);
            }
            if(hospitalDTO.Id>0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Hospital hospital=new Hospital();

            hospital.HospitalName=hospitalDTO.Name;
            hospital.HospitalPhoneNumber=hospitalDTO.PhoneNumber;
            hospital.HospitalAddress=hospitalDTO.Address;
            hospital.State = hospitalDTO.State;

            _context.Hospitals.Add(hospital);
            _context.SaveChanges();
            return CreatedAtRoute("GetHospital", new { id = hospitalDTO.Id }, hospitalDTO);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "DeleteHospital")]

        public ActionResult DeleteHospital(int id)
        {
            if(id<0)
            {
                return BadRequest();
            }
            var hospital = _context.Hospitals.FirstOrDefault(u => u.HospitalId == id);
            if (hospital == null) return NotFound();


            //Remove all departments which are linked to the hospital
            List<Department> departments = _context.Departments.Where(u=>u.HospitalId==id).ToList();
            foreach (var department in departments)
            {
                //Delete more stuff which is connected

                _context.Departments.Remove(department);
                _context.SaveChanges();
            }

            _context.Hospitals.Remove(hospital);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateHospital")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateHospital(int id, [FromBody] HospitalDTO hospitalDTO)
        {
            if (hospitalDTO == null || id != hospitalDTO.Id)
            {
                return BadRequest();
            }

            Hospital hospital=_context.Hospitals.FirstOrDefault(u=>u.HospitalId == id);
            hospital.HospitalName = hospitalDTO.Name;
            hospital.HospitalPhoneNumber = hospitalDTO.PhoneNumber;
            hospital.HospitalAddress= hospitalDTO.Address;
            hospital.State= hospitalDTO.State;  

            _context.Hospitals.Update(hospital);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
