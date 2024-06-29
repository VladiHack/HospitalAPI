
using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Dto
{
    public class PatientDTO
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

    }
}
