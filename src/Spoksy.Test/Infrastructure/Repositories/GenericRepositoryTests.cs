using Spoksy.Domain.Entities;
using Spoksy.Domain.Contracts;
using Spoksy.Infrastructure.Repositories;
using Spoksy.Domain.ValueObjects;

namespace Spoksy.Test.Infrastructure.Repositories
{
    public class GenericRepositoryTests : IntegrationTestBase
    {
        private readonly IGenericRepository<User> _repository;

        public GenericRepositoryTests() : base()
        {
            _repository = new GenericRepository<User>(_context);
        }

        private User CreateUser(string name = "Test")
        {
            return new User(
                name,
                $"{name.ToLower()}@example.com",
                DateTime.UtcNow.AddYears(-25),
                Country.GetByCode("BR"),
                Guid.NewGuid().ToString()
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            var user1 = CreateUser();
            var user2 = CreateUser("Second");
            await _repository.AddAsync(user1);
            await _repository.AddAsync(user2);
            await _unitOfWork.CommitAsync();

            var result = await _repository.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Id == user1.Id);
            Assert.Contains(result, e => e.Id == user2.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingEntity_ShouldReturnEntity()
        {
            var user = CreateUser();
            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _repository.GetByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingEntity_ShouldReturnNull()
        {
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntitySuccessfully()
        {
            var user = CreateUser();

            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var savedEntity = await _repository.GetByIdAsync(user.Id);
            Assert.NotNull(savedEntity);
            Assert.Equal(user.Id, savedEntity.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntitySuccessfully()
        {
            var user = CreateUser();
            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            user.UpdateName("Updated Name");
            await _repository.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            var updatedEntity = await _repository.GetByIdAsync(user.Id);
            Assert.NotNull(updatedEntity);
            Assert.Equal("Updated Name", updatedEntity.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEntitySuccessfully()
        {
            var user = CreateUser();
            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            await _repository.DeleteAsync(user.Id);
            await _unitOfWork.CommitAsync();

            var deletedEntity = await _repository.GetByIdAsync(user.Id);
            Assert.Null(deletedEntity);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingEntity_ShouldReturnTrue()
        {
            var user = CreateUser();
            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var exists = await _repository.ExistsAsync(user.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingEntity_ShouldReturnFalse()
        {
            var exists = await _repository.ExistsAsync(Guid.NewGuid());

            Assert.False(exists);
        }

        [Fact]
        public async Task FindAsync_WithValidCondition_ShouldReturnEntities()
        {
            var user1 = CreateUser();
            var user2 = CreateUser("Second");
            await _repository.AddAsync(user1);
            await _repository.AddAsync(user2);
            await _unitOfWork.CommitAsync();

            var result = await _repository.FindAsync(x => x.Name == user1.Name);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, e => e.Id == user1.Id);
        }

        [Fact]
        public async Task FindAsync_WithInvalidCondition_ShouldReturnEmptyList()
        {
            var user = CreateUser();
            await _repository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            var result = await _repository.FindAsync(x => x.Name == "NonExisting");

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 