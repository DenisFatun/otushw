using DialogsApp.Infrastructure.Repos;
using DialogsApp.Infrastructure.Services;
using DialogsApp.Models.Dialogs;

namespace DialogsApp.Services
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
