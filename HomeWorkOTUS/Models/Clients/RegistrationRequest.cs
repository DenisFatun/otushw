using System.ComponentModel.DataAnnotations;

namespace HomeWorkOTUS.Models.Clients
{
    public class RegistrationRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string SerName { get; set; }

        [Required]
        [Range(0, 100)]
        public int Age { get; set; }

        public bool IsMale { get; set; }

        [MaxLength(300)]
        public string Interests { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
