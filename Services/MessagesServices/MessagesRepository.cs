using Profunion.Interfaces.MessageInterface;
using Profunion.Models;

namespace Profunion.Services.MessagesServices
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly IMessagesRepository _messagesRepository;

        public MessagesRepository(IMessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }
        public async Task<IEnumerable<Messages>> GetMessagesForUserAsync(string userId)
        {
            return await _messagesRepository.GetMessagesForUserAsync(userId);
        }
        
        public async Task<Messages> GetMessageAsync(int messageId)
        {
            return await _messagesRepository.GetMessageAsync(messageId);
        }

        public Task<Messages> SendMessageAsync(string senderId, string recipientId, string messageText)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            throw new NotImplementedException();
        }
    }
}
