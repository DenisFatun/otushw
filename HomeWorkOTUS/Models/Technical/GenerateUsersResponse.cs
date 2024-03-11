namespace HomeWorkOTUS.Models.Technical
{
    public class GenerateUsersResponse
    {
        public int SuccessCount { get; set; }
        public int ErrorsCount { get; set; }
        public List<string>Top10Errors { get; set; } = new List<string>();
    }
}
