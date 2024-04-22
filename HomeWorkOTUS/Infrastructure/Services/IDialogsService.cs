using HomeWorkOTUS.Models.Dialogs;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IDialogsService : IService
    {
        Task SendAsync(Guid to, Guid from, DialogBase dialog);
        Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from);
    }
}
