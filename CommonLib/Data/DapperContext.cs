using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace CommonLib.Data
{
    public class DapperContext : IDapperContext, IDisposable
    {
        private NpgsqlConnection? _sqlConnection;
        private const string GetConnectionStringKey = "PsqlConnection";

        public DapperContext(IConfiguration configuration) 
        {
            var connectionString = configuration.GetConnectionString(GetConnectionStringKey);
            _sqlConnection = new NpgsqlConnection(connectionString);
        }

        public IDbConnection Connection { get => _sqlConnection ?? throw new NullReferenceException($"_sqlConnection of {typeof(DapperContext).Name}"); }

        public void Dispose()
        {
            _sqlConnection?.Dispose();
            _sqlConnection = null;
        }
    }

    public interface IDapperContext
    {
        IDbConnection Connection { get; }
    }

    public enum SqlOrder
    {
        DESC,
        ASC
    }
}
