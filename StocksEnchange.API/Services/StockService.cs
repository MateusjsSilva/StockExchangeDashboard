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
            _logger.LogInformation("Stock service started");
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
                    _logger.LogInformation("Stock service is shutting down");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating stock prices");
                    await Task.Delay(5000, stoppingToken);
                }
            }
            
            _logger.LogInformation("Stock service stopped");
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

                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdates", updates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending price updates");
                throw;
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
            _logger.LogInformation("Stock service is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}