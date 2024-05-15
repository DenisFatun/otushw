using CommonLib.Infrastructure.Services;
using HomeWorkOTUS.Models.Dialogs;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IDialogsService : IService
    {
        Task SendAsync(Guid toClient, DialogBase dialog);
        Task<IEnumerable<Dialog>> ListAsync(Guid from);
    }
}
