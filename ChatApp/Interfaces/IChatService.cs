using ChatApp.Models;

namespace ChatApp.Interfaces
{
    public interface IChatService
    {
        IEnumerable<ChatMessage> GetRecentMessages();
        void AddMessage(ChatMessage message);
    }
}
