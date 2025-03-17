using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace StocksEnchange.API.Hubs
{
    public class StockHub : Hub
    {
        // Armazenar conexões ativas e seus grupos
        private static readonly ConcurrentDictionary<string, HashSet<string>> _connectionGroups = new();
        private readonly ILogger<StockHub> _logger;

        public StockHub(ILogger<StockHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Novo cliente conectado: {connectionId}");
            
            _connectionGroups.TryAdd(connectionId, new HashSet<string>());
            
            await Clients.Caller.SendAsync("Welcome", $"Conectado ao serviço de cotações. Sua conexão ID: {connectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Cliente desconectado: {connectionId}");
            
            _connectionGroups.TryRemove(connectionId, out _);
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeToStock(string stockSymbol)
        {
            var connectionId = Context.ConnectionId;
            
            await Groups.AddToGroupAsync(connectionId, stockSymbol);
            
            if (_connectionGroups.TryGetValue(connectionId, out var groups))
            {
                groups.Add(stockSymbol);
            }
            
            _logger.LogInformation($"Cliente {connectionId} inscrito na ação {stockSymbol}");
            await Clients.Caller.SendAsync("SubscriptionSuccess", $"Inscrito com sucesso na ação {stockSymbol}");
        }

        public async Task UnsubscribeFromStock(string stockSymbol)
        {
            var connectionId = Context.ConnectionId;
            
            await Groups.RemoveFromGroupAsync(connectionId, stockSymbol);
            
            if (_connectionGroups.TryGetValue(connectionId, out var groups))
            {
                groups.Remove(stockSymbol);
            }
            
            _logger.LogInformation($"Cliente {connectionId} desinscrito da ação {stockSymbol}");
        }
    }
}