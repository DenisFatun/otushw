using HomeWorkOTUS.Models.Posts;

namespace HomeWorkOTUS.Models.RabbitMq
{
    public class AddedPost
    {
        public ClientPost Post { get; set; }

        public IEnumerable<Guid> ClientIds { get; set; }
    }
}
