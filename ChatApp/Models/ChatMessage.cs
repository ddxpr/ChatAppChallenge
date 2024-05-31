namespace ChatApp.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
