using Microsoft.AspNetCore.SignalR;

namespace StocksEnchange.API.Hubs
{
    public class StockHub : Hub
    {
        private readonly ILogger<StockHub> _logger;

        public StockHub(ILogger<StockHub> logger)
        {
            _logger = logger;
        }

        // Called when a new client connects
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();
            var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown";
            
            _logger.LogInformation($"New client connected: {connectionId} | UA: {userAgent}");
            
            try
            {
                await Clients.Caller.SendAsync("Welcome", $"Connected to the stock service. Your connection: {connectionId.Substring(0, 8)}...", connectionId);
                _logger.LogInformation($"Welcome message sent to: {connectionId}");
                
                // Send a test message to verify connection
                await Clients.Caller.SendAsync("ReceiveStockUpdates", new[] { 
                    new { 
                        Symbol = "TEST", 
                        Price = 100m, 
                        Timestamp = DateTime.UtcNow,
                        Change = 0m,
                        PercentChange = 0m
                    } 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending welcome message to: {connectionId}");
            }
            
            await base.OnConnectedAsync();
        }

        // Called when a client disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Client disconnected: {connectionId}");
            
            if (exception != null)
            {
                _logger.LogError(exception, $"Error during client disconnection: {connectionId}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
        
        // Add a direct method that clients can call to test connectivity
        public string Ping()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Ping received from client: {connectionId}");
            return $"Pong: {DateTime.UtcNow}";
        }
    }
}