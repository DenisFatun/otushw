using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Dialogs;

namespace HomeWorkOTUS.Services
{
    public class DialogsService : IDialogsService
    {
        private readonly IDialogsRepo _dialogsRepo;
        public DialogsService(IDialogsRepo dialogsRepo)
        {
            _dialogsRepo = dialogsRepo;
        }

        public async Task SendAsync(Guid to, Guid from, DialogBase dialog)
        {
            await _dialogsRepo.CreateAsync(to, from, dialog);            
        }

        public async Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from)
        {
            return await _dialogsRepo.ListAsync(to, from);
        }
    }
}
