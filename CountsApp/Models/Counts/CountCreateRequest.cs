namespace CountsApp.Models.Counts
{
    public class CountCreateRequest
    {
        public Guid To { get; set; }
        public Guid From { get; set; }
        public int DialogId { get; set; }
    }
}
