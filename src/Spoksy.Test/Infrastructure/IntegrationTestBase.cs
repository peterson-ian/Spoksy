using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Spoksy.Infrastructure.Data;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;

namespace Spoksy.Test.Infrastructure
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        protected readonly AppDbContext _context;
        protected readonly IUnitOfWork _unitOfWork;

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
} 
