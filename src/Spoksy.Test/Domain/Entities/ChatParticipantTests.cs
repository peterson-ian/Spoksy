using Spoksy.Domain.Entities;
using Spoksy.Domain.Exceptions;

namespace Spoksy.Test.Domain.Entities
{
    public class ChatParticipantTests
    {
        private readonly Guid _validUserId = Guid.NewGuid();
        private readonly Guid _validChatId = Guid.NewGuid();

        [Fact]
        public void CreateChatParticipant_WithValidData_ShouldCreateSuccessfully()
        {
            var joinAt = DateTime.UtcNow;
            var leaveAt = DateTime.UtcNow.AddHours(1);

            var participant = new ChatParticipant( _validUserId, _validChatId);

            Assert.NotNull(participant);
            Assert.Equal(_validUserId, participant.UserId);
            Assert.Equal(_validChatId, participant.ChatId);
            Assert.NotNull(participant.JoinAt);
            Assert.Null(participant.LeaveAt);
        }

        [Fact]
        public void Leave_ShouldSetLeaveAtToCurrentTime()
        {
            var participant = new ChatParticipant(_validUserId, _validChatId);

            participant.Leave();

            Assert.NotNull(participant.LeaveAt);
            Assert.True(participant.LeaveAt.Value.Kind == DateTimeKind.Utc);
            Assert.True((DateTime.UtcNow - participant.LeaveAt.Value).TotalSeconds < 1);
        }

        [Fact]
        public void CreateChatParticipant_WithEmptyUserId_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new ChatParticipant(Guid.Empty, _validChatId));
            Assert.Equal("User ID cannot be empty", exception.Message);
        }

        [Fact]
        public void CreateChatParticipant_WithEmptyChatId_ShouldThrowDomainException()
        {
            var exception = Assert.Throws<DomainException>(() =>
                new ChatParticipant(_validUserId, Guid.Empty));
            Assert.Equal("Chat ID cannot be empty", exception.Message);
        }
    }
} 