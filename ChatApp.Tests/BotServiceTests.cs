using ChatApp.Interfaces;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ChatApp.Tests
{
    public class BotServiceTests
    {
        [Fact]
        public async Task ProcessCommandAsync_StockCommand_SendsMessageToChatService()
        {            
            var chatServiceMock = new Mock<IChatService>();
            var configurationMock = new Mock<IConfiguration>();

            var botService = new BotService(chatServiceMock.Object, configurationMock.Object);
                        
            await botService.ProcessCommandAsync("/stock=AAPL");
                        
            chatServiceMock.Verify(
                service => service.AddMessage(It.Is<ChatMessage>(m => m.Message.Contains("quote is $"))),
                Times.Once);
        }

        
    }
}
