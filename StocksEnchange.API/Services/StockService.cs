using Microsoft.AspNetCore.SignalR;
using StocksEnchange.API.Hubs;
using StocksEnchange.API.Models;

namespace StocksEnchange.API.Services
{
    public class StockService : BackgroundService
    {
        private readonly IHubContext<StockHub> _hubContext;
        private readonly ILogger<StockService> _logger;
        private readonly Dictionary<string, decimal> _lastPrices;
        private readonly Random _random;
        private readonly string[] _stockSymbols = { "AAPL", "MSFT", "GOOGL", "AMZN", "META", "TSLA", "PETR4", "VALE3", "ITUB4", "BBDC4" };
        private bool _isRunning = false;

        public StockService(IHubContext<StockHub> hubContext, ILogger<StockService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            _random = new Random();
            _lastPrices = _stockSymbols.ToDictionary(s => s, _ => GetRandomPrice());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de cotações iniciado");
            _isRunning = true;
            
            while (!stoppingToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    await UpdateStockPrices();
                    await Task.Delay(1000, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignorar exceções de cancelamento
                    _logger.LogInformation("Serviço de cotações sendo encerrado");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar os preços das ações");
                    await Task.Delay(5000, stoppingToken); // Esperar mais tempo em caso de erro
                }
            }
            
            _logger.LogInformation("Serviço de cotações encerrado");
        }

        private async Task UpdateStockPrices()
        {
            try
            {
                var updates = new List<StockPrice>();
                
                foreach (var symbol in _stockSymbols)
                {
                    var lastPrice = _lastPrices[symbol];
                    var newPrice = GetUpdatedPrice(lastPrice);
                    _lastPrices[symbol] = newPrice;

                    var change = newPrice - lastPrice;
                    var percentChange = (change / lastPrice) * 100;

                    updates.Add(new StockPrice
                    {
                        Symbol = symbol,
                        Price = Math.Round(newPrice, 2),
                        Timestamp = DateTime.UtcNow,
                        Change = Math.Round(change, 2),
                        PercentChange = Math.Round(percentChange, 2)
                    });
                }

                // Enviar todas as atualizações de uma vez para todos os clientes
                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdates", updates);
                
                // Não enviar para grupos específicos para evitar problemas de conexão
                // O cliente pode filtrar os dados que deseja exibir
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar atualizações de preços");
                throw; // Repassar a exceção para ser tratada no método ExecuteAsync
            }
        }

        public Dictionary<string, decimal> GetAllCurrentPrices()
        {
            return _lastPrices.ToDictionary(kvp => kvp.Key, kvp => Math.Round(kvp.Value, 2));
        }

        private decimal GetRandomPrice()
        {
            return (decimal)(_random.NextDouble() * 1000 + 10);
        }

        private decimal GetUpdatedPrice(decimal lastPrice)
        {
            var changePercent = (decimal)(_random.NextDouble() * 2 - 1) / 100;
            var change = lastPrice * changePercent;
            return Math.Max(1, lastPrice + change);
        }
        
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            _logger.LogInformation("Serviço de cotações está sendo interrompido");
            await base.StopAsync(cancellationToken);
        }
    }
}
