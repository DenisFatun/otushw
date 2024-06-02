using CommonLib.Infrastructure.Services;

namespace CountsApp.Infrastructure.Services
{
    public interface IDialogsCountService : IService
    {
        Task CreateAsync(Guid to, Guid from, int dialogId);
        Task UpdateAsync(Guid to, Guid from, int currentLastRead);
        Task<int> GetCountAsync(Guid to, Guid from);
    }
}
