using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace StocksEnchange.API.Hubs
{
    public class StockHub : Hub
    {
        // Called when a new client connects
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();
            var userAgent = httpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown";
            
            Log.Information("New client connected: {ConnectionId} | UA: {UserAgent}", connectionId, userAgent);
            
            try
            {
                await Clients.Caller.SendAsync("Welcome", $"Connected to the stock service. Your connection: {connectionId.Substring(0, 8)}...", connectionId);
                Log.Information("Welcome message sent to: {ConnectionId}", connectionId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending welcome message to: {ConnectionId}", connectionId);
            }
            
            await base.OnConnectedAsync();
        }

        // Called when a client disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            Log.Information("Client disconnected: {ConnectionId}", connectionId);
            
            if (exception != null)
            {
                Log.Error(exception, "Error during client disconnection: {ConnectionId}", connectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
        
        // Add a direct method that clients can call to test connectivity
        public string Ping()
        {
            var connectionId = Context.ConnectionId;
            Log.Information("Ping received from client: {ConnectionId}", connectionId);
            return $"Pong: {DateTime.UtcNow}";
        }
    }
}