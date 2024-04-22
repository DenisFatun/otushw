using CommonLib.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;

namespace HomeWorkOTUS.Services.SignalR
{
    public class UserHandlerData
    {
        public Guid ClientId { get; set; }
        public string ConnectionId { get; set; }
    }

    public interface IFeedPostedHub
    {
        Task SentPostToFriendsAsync(string post, List<Guid> friends);
    }

    public class FeedPostedHub : Hub<IFeedPostedHub>
    {
        public static List<UserHandlerData> OnlineUsers = new List<UserHandlerData>();

        private readonly IJWTService _jwtService;

        public FeedPostedHub(IJWTService jwtService) 
        {
            _jwtService = jwtService;
        }

        public override Task OnConnectedAsync()
        {
            var token = Context.GetHttpContext().Request.Query["access_token"];
            var tokenClaims = _jwtService.ValidateToken(token);

            if (tokenClaims != null)
            {
                OnlineUsers.Add(new UserHandlerData { ClientId = tokenClaims.ClientId, ConnectionId = Context.ConnectionId });
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var onlineUser = OnlineUsers.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (onlineUser != null)
                OnlineUsers.Remove(onlineUser);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
