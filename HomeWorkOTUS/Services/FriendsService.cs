using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Infrastructure.Services;

namespace HomeWorkOTUS.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendsRepo _friendsRepo;
        public FriendsService(IFriendsRepo friendsRepo)
        {
            _friendsRepo = friendsRepo;
        }

        public async Task AddFriendAsync(Guid clientId, Guid friendId)
        {
            await _friendsRepo.CreateAsync(clientId, friendId);
        }

        public async Task RemoveFriendAsync(Guid clientId, Guid friendId)
        {
            await _friendsRepo.DeleteAsync(clientId, friendId);
        }
    }
}
