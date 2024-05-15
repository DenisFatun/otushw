using CommonLib.Infrastructure.Repos;
using DialogsApp.Models.Dialogs;

namespace DialogsApp.Infrastructure.Repos
{
    public interface IDialogsRepo : IRepo
    {
        Task<int> CreateAsync(Guid to, Guid from, DialogBase dialog);
        Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from);
    }
}
