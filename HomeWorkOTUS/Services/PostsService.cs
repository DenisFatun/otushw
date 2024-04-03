using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Posts;
using HomeWorkOTUS.Models.RabbitMq;
using MassTransit;
using System.Security.Claims;
using System.Text.Json;

namespace HomeWorkOTUS.Services
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepo _postsRepo;
        private readonly IFriendsRepo _friendsRepo;
        private readonly IRedisService _redisService;
        private readonly IClientsRepo _clientsRepo;
        private readonly IBusControl _rabbitBusControl;

        public PostsService(IPostsRepo postsRepo, 
            IFriendsRepo friendsRepo,
            IRedisService redisService,
            IClientsRepo clientsRepo,
            IBusControl rabbitBusControl)
        {
            _postsRepo = postsRepo;
            _friendsRepo = friendsRepo;
            _redisService= redisService;
            _clientsRepo = clientsRepo;
            _rabbitBusControl = rabbitBusControl;
        }

        public async Task AddPostAsync(Guid clientId, string text)
        {
            var postId = await _postsRepo.CreateAsync(clientId, text);
            var friends = await _friendsRepo.ListAsync(clientId);
            if (friends.Any())
            {
                var post = await _postsRepo.GetAsync(postId);

                using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    await _rabbitBusControl.Publish(new AddedPost { Post = post, ClientIds = friends }, source.Token);
                }

                foreach (var friend in friends)
                    await _redisService.SaveListAsync(friend.ToString(), JsonSerializer.Serialize(post));
            }
        }

        public async Task RemovePostAsync(Guid clientId, int postId)
        {
            await _postsRepo.DeleteAsync(clientId, postId);
            var friends = await _friendsRepo.ListAsync(clientId);
            await UpdatePostCacheAsync(friends.ToList());
        }
            
        public async Task UpdatePostAsync(Guid clientId, int postId, string text)
        {
            await _postsRepo.UpdateAsync(clientId, postId, text);
            var friends = await _friendsRepo.ListAsync(clientId);
            await UpdatePostCacheAsync(friends.ToList());
        }

        public async Task<ClientPost> GetPostAsync(int postId)
        {
            return await _postsRepo.GetAsync(postId);
        }

        public async Task<IEnumerable<ClientPost>> ListPostFromCacheAsync(Guid clientId, PostListFilter filter)
        {
            var result = await _redisService.ListAsync(clientId.ToString(), filter);
            return result.Select(x => JsonSerializer.Deserialize<ClientPost>(x));
        }

        public async Task<IEnumerable<ClientPost>> ListPostAsync(Guid clientId, PostListFilter filter)
        {
            return await _postsRepo.ListAsync(clientId, filter);
        }

        public async Task<int> UpdatePostCacheAsync(List<Guid> clientIdList)
        {
            int i = 0;
            if (clientIdList != null && clientIdList.Count > 0)
            {
                foreach (var id in clientIdList)
                {
                    await ClientPostsCacheAsync(id);
                    i++;
                }
            }
            else
            {
                foreach (var id in (await _clientsRepo.GetClientIdAllAsync()).ToList())
                {
                    await ClientPostsCacheAsync(id);
                    i++;
                }
            }
            return i;
        }

        private async Task ClientPostsCacheAsync(Guid clientId)
        {
            await _redisService.RemoveListAsync(clientId.ToString());
            var posts = await _postsRepo.ListAsync(clientId, new PostListFilter { Offset = 0, Limit = 100 }, Data.SqlOrder.ASC);
            foreach (var post in posts)
                await _redisService.SaveListAsync(clientId.ToString(), JsonSerializer.Serialize(post));            
        }
    }
}
