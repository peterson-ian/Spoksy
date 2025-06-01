namespace Spoksy.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task RollbackAsync();
        void Dispose();
    }
}