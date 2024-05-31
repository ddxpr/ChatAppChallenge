using ChatApp.Data;
using ChatApp.Interfaces;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ChatMessage> GetRecentMessages()
        {
            return _context.ChatMessages.OrderByDescending(m => m.Timestamp).Take(50).ToList();
        }

        public void AddMessage(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            _context.SaveChanges();
        }
    }
}
