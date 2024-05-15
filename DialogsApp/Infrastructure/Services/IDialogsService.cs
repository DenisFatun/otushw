using CommonLib.Infrastructure.Services;
using DialogsApp.Models.Dialogs;

namespace DialogsApp.Infrastructure.Services
{
    public interface IDialogsService : IService
    {
        Task SendAsync(Guid to, Guid from, DialogBase dialog);
        Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from);
    }
}
