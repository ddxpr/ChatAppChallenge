using System.Collections.Generic;
using System.Linq;
using ChatApp.Data;
using ChatApp.Interfaces;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ChatMessage> GetRecentMessages(int chatRoomId)
        {
            return _context.ChatMessages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .ToList();
        }

        public void AddMessage(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            _context.SaveChanges();
        }

        public IEnumerable<ChatRoom> GetChatRooms()
        {
            return _context.ChatRooms.ToList();
        }

        public ChatRoom GetChatRoom(int id)
        {
            return _context.ChatRooms.Include(r => r.Messages).FirstOrDefault(r => r.Id == id);
        }
    }
}
