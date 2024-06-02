using CommonLib.Infrastructure.Services;

namespace DialogsApp.Infrastructure.Services
{
    public interface ICountsService : IService
    {
        Task CreateAsync(Guid to, Guid from, int dialogId);
        Task UpdateAsync(Guid to, Guid from, int lastReadId);
    }
}
