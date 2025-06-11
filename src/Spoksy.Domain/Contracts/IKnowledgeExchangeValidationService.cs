using Spoksy.Domain.ValueObjects;

namespace Spoksy.Domain.Contracts
{
    public interface IKnowledgeExchangeValidationService
    {
        Task EnsureKnowledgeExchangeAsync(Guid firstUserId, Guid secondUserId, Language languageA, Language languageB);
    }
}
