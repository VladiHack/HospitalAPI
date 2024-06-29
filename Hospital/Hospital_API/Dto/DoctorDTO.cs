using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Dto
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(16)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(16)]
        public string LastName { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }
    }
}
