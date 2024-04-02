using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.RabbitMq;
using MassTransit;

namespace HomeWorkOTUS.Services.RabbitMq
{
    public class AddPostConsumer : IConsumer<AddPost>
    {
        private readonly IPostsService _postService;
        public AddPostConsumer(IPostsService postService)
        {
            _postService = postService;
        }

        public async Task Consume(ConsumeContext<AddPost> context)
        {
            await _postService.AddPostAsync(context.Message.ClientId, context.Message.Text);
        }
    }
}
