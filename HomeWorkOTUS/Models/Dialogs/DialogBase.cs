using System.ComponentModel.DataAnnotations;

namespace HomeWorkOTUS.Models.Dialogs
{
    public class DialogBase
    {
        [Required]
        public string Message { get; set; }
    }
}
