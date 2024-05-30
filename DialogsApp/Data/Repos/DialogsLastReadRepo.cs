using CommonLib.Data;
using Dapper;
using DialogsApp.Infrastructure.Repos;

namespace DialogsApp.Data.Repos
{
    public class DialogsLastReadRepo : IDialogsLastReadRepo
    {
        private readonly IDapperContext _db;

        public DialogsLastReadRepo(IDapperContext db)
        {
            _db = db;
        }

        public async Task<int?> GetAsync(Guid to, Guid from)
        {
            var sql = $@"SELECT last_read
                        FROM dialogs_last_read
                        WHERE to_client_id = @to AND from_client_id = @from";

            return await _db.Connection.QueryFirstOrDefaultAsync<int?>(sql,
            new
            {
                to,
                from
            });
        }

        public async Task UpdateAsync(Guid to, Guid from, int lastRead)
        {
            var sql = $@"Update dialogs_last_read
                        SET last_read = @lastRead
                        WHERE to_client_id = @to AND from_client_id = @from";

            await _db.Connection.ExecuteAsync(sql,
            new
            {
                to,
                from,
                lastRead
            });
        }

        public async Task CreateAsync(Guid to, Guid from)
        {
            var sql = $@"INSERT INTO dialogs_last_read (to_client_id, from_client_id, last_read) 
                        VALUES (@to, @from, 0)";

            await _db.Connection.ExecuteAsync(sql,
            new
            {
                to,
                from
            });
        }
    }
}
