namespace Spoksy.Domain.Contracts
{
    public interface IChatValidationService
    {
        Task EnsureChatAvailableAsync(Guid chatId);
        Task EnsureUserAccessForChatAsync(Guid userId, Guid chatId);

    }
}
