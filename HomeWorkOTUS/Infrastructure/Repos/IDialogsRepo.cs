using HomeWorkOTUS.Models.Dialogs;

namespace HomeWorkOTUS.Infrastructure.Repos
{
    public interface IDialogsRepo : IRepo
    {
        Task<int> CreateAsync(Guid to, Guid from, DialogBase dialog);
        Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from);
    }
}
