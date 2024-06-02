using CommonLib.Data;
using DialogsApp.Infrastructure.Repos;
using DialogsApp.Infrastructure.Services;
using DialogsApp.Models.Dialogs;

namespace DialogsApp.Services
{
    public class DialogsService : IDialogsService
    {
        private readonly IDialogsRepo _dialogsRepo;
        private readonly IDapperContext _db;
        private readonly ICountsService _countsService;
        public DialogsService(IDialogsRepo dialogsRepo,
            IDapperContext db,
            ICountsService countsService)
        {
            _dialogsRepo = dialogsRepo;
            _db = db;
            _countsService = countsService;
        }

        public async Task SendAsync(Guid to, Guid from, DialogBase dialog)
        {
            _db.Connection.Open();
            using var transaction = _db.Connection.BeginTransaction();
            try
            {                
                var dialogId = await _dialogsRepo.CreateAsync(to, from, dialog);
                await _countsService.CreateAsync(to, from, dialogId);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            transaction.Commit();
        }

        public async Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from)
        {
            var result = await _dialogsRepo.ListAsync(to, from);
            var lastReadId = result.Max(x => x.Id);
            await _countsService.UpdateAsync(to, from, lastReadId);
            return result;
        }
    }
}
