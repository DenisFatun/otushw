namespace HomeWorkOTUS.Infrastructure.Repos
{
    public interface IFriendsRepo : IRepo
    {
        Task<int> CreateAsync(Guid clientId, Guid friendId);
        Task DeleteAsync(Guid clientId, Guid friendId);
        Task<IEnumerable<Guid>> ListAsync(Guid clientId);
    }
}
