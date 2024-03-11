using System.ComponentModel.DataAnnotations;

namespace HomeWorkOTUS.Models.Clients
{
    public class LoginRequest
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
