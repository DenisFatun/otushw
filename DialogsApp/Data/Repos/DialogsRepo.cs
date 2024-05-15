using CommonLib.Data;
using Dapper;
using DialogsApp.Infrastructure.Repos;
using DialogsApp.Models.Dialogs;

namespace DialogsApp.Data.Repos
{
    public class DialogsRepo : IDialogsRepo
    {
        private readonly IDapperContext _db;

        public DialogsRepo(IDapperContext db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Guid to, Guid from, DialogBase dialog)
        {
            var sql = $@"INSERT INTO dialogs (to_client_id, from_client_id, message) 
                        VALUES (@to, @from, @message) 
                        returning id";

            return await _db.Connection.ExecuteScalarAsync<int>(sql,
            new
            {
                to,
                from,
                message = dialog.Message
            });
        }

        public async Task<IEnumerable<Dialog>> ListAsync(Guid to, Guid from)
        {
            var sql = $@"SELECT id as Id,
                            to_client_id as ClientId,
                            from_client_id as From,
                            message as Message,
                            created_at as Created
                        FROM dialogs
                        WHERE to_client_id = @to AND from_client_id = @from";

            return await _db.Connection.QueryAsync<Dialog>(sql,
            new
            {
                to,
                from
            });
        }
    }
}
