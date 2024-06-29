using System.ComponentModel.DataAnnotations;

namespace Hospital_API.Dto
{
    public class HospitalDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [MaxLength(10)]  
        public string PhoneNumber { get; set; }
        [Required]
        public string State { get; set; }
    }
}
