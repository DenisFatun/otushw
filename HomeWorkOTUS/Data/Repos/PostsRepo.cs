using Dapper;
using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Models.Posts;

namespace HomeWorkOTUS.Data.Repos
{
    public class PostsRepo //: IPostsRepo
    {
        private readonly IDapperContext _db;

        public PostsRepo(IDapperContext db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Guid clientId, string text)
        {
            var sql = $@"INSERT INTO posts (client_id, post_text) 
                        VALUES (@clientId, @text) 
                        returning id";

            return await _db.Connection.ExecuteScalarAsync<int>(sql,
            new
            {
                clientId,
                text
            });
        }

        public async Task DeleteAsync(Guid clientId, int id)
        {
            var sql = $@"DELETE FROM posts
                        WHERE id = @id AND client_id = @clientId";

            await _db.Connection.ExecuteAsync(sql,
            new
            {
                id,
                clientId
            });
        }

        public async Task UpdateAsync(Guid clientId, int id, string text)
        {
            var sql = $@"UPDATE posts SET post_text = @text
                        WHERE id = @id AND client_id = @clientId";

            await _db.Connection.ExecuteAsync(sql,
            new
            {
                id,
                clientId,
                text
            });
        }

        public async Task<ClientPost> GetAsync(int id)
        {
            var sql = $@"SELECT
                            p.id as Id,
                            c.name||' '||c.ser_name as Author,
                            p.post_text as Text,
                            p.created_at as Created
                        FROM posts p
                        INNER JOIN clients c ON c.id = p.client_id
                        WHERE p.id = @id
                        LIMIT 1";

            return await _db.Connection.QueryFirstOrDefaultAsync<ClientPost>(sql,
            new
            {
                id
            });
        }

        public async Task<IEnumerable<ClientPost>> ListAsync(Guid clientId, PostListFilter filter, SqlOrder order = SqlOrder.DESC)
        {
            var sql = $@"SELECT
                            p.id as Id,
                            c.name||' '||c.ser_name as Author,
                            p.post_text as Text,
                            p.created_at as Created
                        FROM posts p
                        INNER JOIN clients c ON c.id = p.client_id
                        WHERE p.client_id IN (SELECT cf.friend_id FROM clients_friends cf WHERE cf.client_id = @clientId)
                        ORDER BY ID {order}
                        LIMIT @limit OFFSET @offset";

            return await _db.Connection.QueryAsync<ClientPost>(sql,
            new
            {
                clientId,
                limit = filter.Limit,
                offset = filter.Offset
            });
        }

        public async Task<IEnumerable<ClientPostSimple>> PostsByAuthorAsync(Guid clientId)
        {
            var sql = $@"SELECT
                            p.id as Id,
                            p.client_id as Author,
                            p.post_text as Text,
                            p.created_at as Created
                        FROM posts p
                        WHERE p.client_id = @clientId";

            return await _db.Connection.QueryAsync<ClientPostSimple>(sql,
            new
            {
                clientId
            });
        }
    }
}
