using ChatApp.Interfaces;
using ChatApp.Models;
using RabbitMQ.Client;

namespace ChatApp.Services
{
    public class BotService : IBotService
    {
        private readonly IChatService _chatService;
        private readonly IConfiguration _config;
        private readonly IModel _channel;

        public BotService(IChatService chatService, IConfiguration config)
        {
            _chatService = chatService;
            _config = config;

            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQ:HostName"],
                Port = int.Parse(_config["RabbitMQ:Port"]),
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "stock_quotes",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public async Task ProcessCommandAsync(string message)
        {
            if (message.StartsWith("/stock="))
            {
                var stockCode = message.Substring(7).Trim();
                var stockQuote = await GetStockQuote(stockCode);
                if (stockQuote != null)
                {
                    var formattedMessage = $"{stockQuote.Symbol} quote is ${stockQuote.Price} per share.";
                    var botMessage = new ChatMessage
                    {
                        UserId = "bot",
                        Username = "Bot",
                        Message = formattedMessage,
                        Timestamp = DateTime.UtcNow
                    };

                    _chatService.AddMessage(botMessage);

                    // Send message to RabbitMQ
                    var body = System.Text.Encoding.UTF8.GetBytes(formattedMessage);
                    _channel.BasicPublish(exchange: "",
                                          routingKey: "stock_quotes",
                                          basicProperties: null,
                                          body: body);
                }
                else
                {
                    // Handle case where stock quote is not found
                    var errorMessage = $"Stock code '{stockCode}' not found.";
                    var botMessage = new ChatMessage
                    {
                        UserId = "bot",
                        Username = "Bot",
                        Message = errorMessage,
                        Timestamp = DateTime.UtcNow
                    };

                    _chatService.AddMessage(botMessage);
                }
            }
        }

        private async Task<StockQuote> GetStockQuote(string stockCode)
        {
            try
            {
                var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var csvData = await response.Content.ReadAsStringAsync();
                        var lines = csvData.Split('\n');
                        if (lines.Length >= 2)
                        {
                            var dataLine = lines[1].Trim();
                            var values = dataLine.Split(',');
                            if (values.Length >= 6)
                            {
                                var symbol = values[0];
                                var price = decimal.Parse(values[6]);
                                var exchange = values[5];
                                return new StockQuote
                                {
                                    Symbol = symbol,
                                    Price = price,
                                    Exchange = exchange
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error fetching stock quote: {ex.Message}");
                return null;
            }
        }
    }
}
