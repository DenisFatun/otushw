using HomeWorkOTUS.Models.RabbitMq;
using HomeWorkOTUS.Services.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace HomeWorkOTUS.Services.RabbitMq
{
    public class AddedPostConsumer : IConsumer<AddedPost>
    {
        private readonly IHubContext<FeedPostedHub> _hubContext;

        public AddedPostConsumer(IHubContext<FeedPostedHub> hubContext) 
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<AddedPost> context)
        {
            var allConnectionIdForUser = FeedPostedHub.OnlineUsers
                .Where(x => context.Message.ClientIds.Any(y => y == x.ClientId)).Select(x => x.ConnectionId).ToList();

            await _hubContext.Clients.Clients(allConnectionIdForUser).SendAsync("Posted", context.Message.Post);
        }
    }
}
