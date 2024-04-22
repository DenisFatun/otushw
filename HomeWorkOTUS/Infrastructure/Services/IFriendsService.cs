using CommonLib.Infrastructure.Services;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IFriendsService : IService
    {
        Task AddFriendAsync(Guid clientId, Guid friendId);
        Task RemoveFriendAsync(Guid clientId, Guid friendId);
    }
}
