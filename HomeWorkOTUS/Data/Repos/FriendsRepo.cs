using CommonLib.Data;
using Dapper;
using HomeWorkOTUS.Infrastructure.Repos;

namespace HomeWorkOTUS.Data.Repos
{
    public class FriendsRepo : IFriendsRepo
    {
        private readonly IDapperContext _db;

        public FriendsRepo(IDapperContext db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Guid clientId, Guid friendId)
        {
            var sql = $@"INSERT INTO clients_friends (client_id, friend_id) 
                        VALUES (@clientId, @friendId) 
                        returning id";

            return await _db.Connection.ExecuteScalarAsync<int>(sql,
            new
            {
                clientId,
                friendId
            });
        }

        public async Task DeleteAsync(Guid clientId, Guid friendId)
        {
            var sql = $@"DELETE FROM clients_friends
                        WHERE client_id = @clientId AND friend_id = @friendId";                        

            await _db.Connection.ExecuteAsync(sql,
            new
            {
                clientId,
                friendId
            });
        }

        public async Task<IEnumerable<Guid>> ListAsync(Guid clientId)
        {
            var sql = $@"SELECT client_id as friendId
                        FROM clients_friends
                        WHERE friend_id = @clientId";

            return await _db.Connection.QueryAsync<Guid>(sql,
            new
            {
                clientId
            });
        }
    }
}
