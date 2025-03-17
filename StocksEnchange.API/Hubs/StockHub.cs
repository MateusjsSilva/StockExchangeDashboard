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

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Novo cliente conectado: {connectionId}");
            
            try
            {
                await Clients.Caller.SendAsync("Welcome", $"Conectado ao serviço de cotações. Sua conexão: {connectionId.Substring(0, 8)}...");
                _logger.LogInformation($"Mensagem de boas-vindas enviada para: {connectionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar mensagem de boas-vindas para: {connectionId}");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation($"Cliente desconectado: {connectionId}");
            
            if (exception != null)
            {
                _logger.LogError(exception, $"Erro na desconexão do cliente: {connectionId}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}