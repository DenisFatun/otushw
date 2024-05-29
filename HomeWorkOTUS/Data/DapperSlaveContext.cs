using Npgsql;
using System.Data;

namespace HomeWorkOTUS.Data
{
    public class DapperSlaveContext : IDapperSlaveContext, IDisposable
    {
        private NpgsqlConnection? _sqlConnection;
        private const string GetConnectionStringKey = "PsqlConnectionSlave";

        public DapperSlaveContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString(GetConnectionStringKey);
            _sqlConnection = new NpgsqlConnection(connectionString);
        }

        public IDbConnection Connection { get => _sqlConnection ?? throw new NullReferenceException($"_sqlConnection of {typeof(DapperSlaveContext).Name}"); }

        public void Dispose()
        {
            _sqlConnection?.Dispose();
            _sqlConnection = null;
        }
    }

    public interface IDapperSlaveContext
    {
        IDbConnection Connection { get; }
    }
}
