namespace CountsApp.Models.Counts
{
    public class CountUpdateRequest
    {
        public Guid To { get; set; }
        public Guid From { get; set; }
        public int LastReadId { get; set; }
    }
}
