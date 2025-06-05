using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Spoksy.Infrastructure.Data;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using System.Data;
using Dapper;

namespace Spoksy.Test.Infrastructure
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        protected readonly AppDbContext _context;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IDbConnectionFactory _dbConnectionFactory;

        protected IntegrationTestBase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            _context = new AppDbContext(options);
            _unitOfWork = new UnitOfWork(_context);
            _dbConnectionFactory = new TestConnectionFactory(_connection);
            Dapper.SqlMapper.AddTypeHandler(new SqliteGuidTypeHandler());
        }

        public virtual async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public virtual async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
    public class TestConnectionFactory : IDbConnectionFactory
    {
        private readonly SqliteConnection _connection;

        public TestConnectionFactory(SqliteConnection connection)
        {
            _connection = connection;
        }

        public IDbConnection CreateConnection()
        {
            return _connection;
        }
    }

    public class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
            parameter.DbType = DbType.String;
        }

        public override Guid Parse(object value)
        {
            return Guid.Parse(value.ToString());
        }
    }
} 
