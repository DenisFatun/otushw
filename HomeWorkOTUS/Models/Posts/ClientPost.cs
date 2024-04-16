namespace HomeWorkOTUS.Models.Posts
{
    public class ClientPost
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }

    public class ClientPostSimple : ClientPost
    {
        public new Guid Author { get; set; }
    }
}
