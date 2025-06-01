using Microsoft.Extensions.Configuration;
using Npgsql;
using Spoksy.Domain.Contracts;
using System.Data;

namespace Spoksy.Infrastructure.Data
{
    public class AppConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public AppConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
