using CommonLib.Data;
using CommonLib.Extensions;
using Dapper;
using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Models.Clients;

namespace HomeWorkOTUS.Data.Repos
{
    public class ClientsRepo : IClientsRepo
    {
        private readonly IConfiguration _configuration;
        private readonly IDapperContext _db;
        private readonly IDapperSlaveContext _slaveContext;

        private static readonly string Columns = "id, name, ser_name as SerName, age, is_male as IsMale, interests, city, creationdate as CreationDate";

        public ClientsRepo(
            IConfiguration configuration, IDapperContext db, IDapperSlaveContext slaveContext)
        {
            _configuration = configuration;
            _db = db;
            _slaveContext = slaveContext;
        }

        public async Task<Guid> CreateAsync(RegistrationRequest request)
        {
            var sql = $@"insert into clients (id, name, ser_name, age, is_male, interests, city, creationdate, password) 
                        values (@id, @name, @ser_name, @age, @is_male, @interests, @city, @creationDate, @password) 
                        returning id";
            return await _db.Connection.ExecuteScalarAsync<Guid>(sql,
            new
            {
                id = Guid.NewGuid(),
                name = request.Name,
                ser_name = request.SerName,
                age = request.Age,
                is_male = request.IsMale,
                interests = request.Interests,
                city = request.City,
                creationDate = DateTime.Now.ToUniversalTime(),
                password = HashPassword(request.Password)
            });
        }

        public async Task<Client> GetByLoginAndPasswordAsync(Guid id, string password)
        {
            var passwordHash = HashPassword(password);

            var sqlCommand = $@"select {Columns} 
                                from clients
                                where id = @id AND password = @passwordHash
                                limit 1";

            return await _db.Connection.QueryFirstOrDefaultAsync<Client>(sqlCommand, new { id, passwordHash });            
        }

        public async Task<Client> GetAsync(Guid id)
        {
            var sqlCommand = $@"select {Columns}
                                from clients
                                where id = @id
                                limit 1";

            return await _db.Connection.QueryFirstOrDefaultAsync<Client>(sqlCommand, new { id });
        }

        public async Task<ClientSearchResponse> SearchAsync(ClientSearchFilter filter)
        {
            var response = new ClientSearchResponse();
            var sqlCommand = $@"select {Columns}
                                from clients
                                where ser_name like @serName
                                and name like @name
                                order by id";

            response.Items = await _slaveContext.Connection.QueryAsync<Client>(sqlCommand, new { name = filter.Name + "%", serName = filter.SerName + "%" });
            return response;
        }

        public async Task<IEnumerable<Guid>> GetClientIdAllAsync()
        {
            var sqlCommand = $@"select id from clients";
            return await _db.Connection.QueryAsync<Guid>(sqlCommand);
        }

        private string HashPassword(string password)
        {
            return CommonFunc.HashSHA512(_configuration["HashSHA512"] + password);
        }
    }
}
