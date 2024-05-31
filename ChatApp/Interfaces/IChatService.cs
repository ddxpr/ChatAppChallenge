using System.Collections.Generic;
using ChatApp.Models;

namespace ChatApp.Interfaces
{
    public interface IChatService
    {
        IEnumerable<ChatMessage> GetRecentMessages(int chatRoomId);
        void AddMessage(ChatMessage message);
        IEnumerable<ChatRoom> GetChatRooms();
        ChatRoom GetChatRoom(int id);
    }
}
