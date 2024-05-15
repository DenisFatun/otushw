using System.Text.Json.Serialization;

namespace DialogsApp.Models.Dialogs
{
    public class Dialog : DialogBase
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Guid ClientId { get; set; }
        public Guid From { get; set; }
        public DateTime Created { get; set; }
    }
}
