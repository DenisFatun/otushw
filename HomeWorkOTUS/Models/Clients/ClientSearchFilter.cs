using System.ComponentModel.DataAnnotations;

namespace HomeWorkOTUS.Models.Clients
{
    public class ClientSearchFilter
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string SerName { get; set; }
    }
}
