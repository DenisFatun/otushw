using CountsApp.Infrastructure.Repos;
using CountsApp.Infrastructure.Services;

namespace CountsApp.Services
{
    public class DialogsCountService : IDialogsCountService
    {
        private readonly IDialogsLastReadRepo _dialogsLastReadRepo;
        private readonly IRedisService _redisService;
        public DialogsCountService(IDialogsLastReadRepo dialogsLastReadRepo,
            IRedisService redisService) 
        {
            _dialogsLastReadRepo = dialogsLastReadRepo;
            _redisService = redisService;
        }

        public async Task CreateAsync(Guid to, Guid from, int dialogId)
        {
            var key = GetKey(to, from);
            var values = await _redisService.GetAsync(key);
            if (values == null)
            {
                var lastRead = await _dialogsLastReadRepo.GetAsync(to, from);
                if (lastRead == null)
                {
                    await _dialogsLastReadRepo.CreateAsync(to, from);
                }
                await _redisService.SaveAsync(key, [dialogId]);
            }
            else
                await _redisService.SaveAsync(key, values.Concat([dialogId]).ToArray());
        }

        public async Task UpdateAsync(Guid to, Guid from, int lastRead)
        {
            var key = GetKey(to, from);
            await _dialogsLastReadRepo.UpdateAsync(to, from, lastRead);
            var values = await _redisService.GetAsync(key);
            if (values != null)
            {
                var updatedValues = values.ToList().Where(x => x > lastRead).ToArray();
                await _redisService.SaveAsync(key, updatedValues);
            }
        }

        public async Task<int> GetCountAsync(Guid to, Guid from)
        {
            var key = GetKey(to, from);
            var values = await _redisService.GetAsync(key);
            if (values == null)
                return 0;
            else
                return values.Count();
        }

        private string GetKey(Guid to, Guid from)
        {
            return $"{to}-{from}";
        }
    }
}
