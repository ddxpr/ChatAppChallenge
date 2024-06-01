using ChatApp.Data;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChatApp.Tests
{
    public class ChatServiceTests
    {
        [Fact]
        public void GetRecentMessages_ReturnsMessagesInDescendingOrder()
        {           
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_ChatApp")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var service = new ChatService(context);
                                
                var messages = service.GetRecentMessages();
                                
                Assert.Equal(50, messages.Count()); // Ensure it returns up to 50 messages
                Assert.Equal("Message 50", messages.First().Message); // Assuming messages are seeded
            }
        }

        
    }
}
