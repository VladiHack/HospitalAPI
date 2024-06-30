using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Dto
{
    public class DepartmentDTO 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int HospitalId { get; set; }

    }
}
