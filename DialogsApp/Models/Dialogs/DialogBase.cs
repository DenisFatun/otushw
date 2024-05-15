using System.ComponentModel.DataAnnotations;

namespace DialogsApp.Models.Dialogs
{
    public class DialogBase
    {
        [Required]
        public string Message { get; set; }
    }
}
