using CommonLib.Data;
using CommonLib.Model.Rabbit;
using DialogsApp.Infrastructure.Repos;
using DialogsApp.Infrastructure.Services;
using DialogsApp.Models.Dialogs;
using MassTransit;

namespace DialogsApp.Services
{
    public class DialogsService : IDialogsService
    {
        private readonly IDialogsRepo _dialogsRepo;
        private readonly IDialogsLastReadRepo _dialogsLastReadRepo;
        private readonly IDapperContext _db;
        private readonly IBusControl _rabbitBusControl;
        public DialogsService(IDialogsRepo dialogsRepo, 
            IDialogsLastReadRepo dialogsLastReadRepo,
            IDapperContext db,
            IBusControl rabbitBusControl
            )
        {
            _dialogsRepo = dialogsRepo;
            _dialogsLastReadRepo = dialogsLastReadRepo;
            _db = db;
            _rabbitBusControl = rabbitBusControl;
        }

        public async Task SendAsync(Guid to, Guid from, DialogBase dialog)
        {
            _db.Connection.Open();
            using var transaction = _db.Connection.BeginTransaction();
            var dialogId = await _dialogsRepo.CreateAsync(to, from, dialog);            

            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                await _rabbitBusControl.Publish(new DialogCount { 
                    To = to, 
                    From = from, 
                    IsCreated = true, 
                    DialogsId = [dialogId] }
                , source.Token);
            }

            transaction.Commit();
        }

        public async Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from)
        {
            var result = await _dialogsRepo.ListAsync(to, from);

            var lastRead = await _dialogsLastReadRepo.GetAsync(to, from);

            if (lastRead == null)
            {
                await _dialogsLastReadRepo.CreateAsync(to, from);
                lastRead = 0;
            }

            var currentLastRead = result.Max(x => x.Id);
            if (result.Any() && currentLastRead > lastRead)
            {
                await _dialogsLastReadRepo.UpdateAsync(to, from, currentLastRead);
                using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    await _rabbitBusControl.Publish(new DialogCount { 
                        To = to, 
                        From = from, 
                        IsCreated = false, 
                        DialogsId = result.Select(x => x.Id).ToArray(), 
                        LastRead = currentLastRead }
                    , source.Token);
                }

            }            

            return result;
        }
    }
}
