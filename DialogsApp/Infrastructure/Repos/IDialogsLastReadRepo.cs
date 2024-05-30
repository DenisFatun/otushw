using CommonLib.Infrastructure.Repos;

namespace DialogsApp.Infrastructure.Repos
{
    public interface IDialogsLastReadRepo : IRepo
    {
        Task<int?> GetAsync(Guid to, Guid from);
        Task UpdateAsync(Guid to, Guid from, int lastRead);
        Task CreateAsync(Guid to, Guid from);
    }
}
