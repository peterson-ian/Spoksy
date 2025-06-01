using System.Data;

namespace Spoksy.Domain.Contracts
{
    public interface IDbConnectionFactory
    {
            IDbConnection CreateConnection();
    }
}
