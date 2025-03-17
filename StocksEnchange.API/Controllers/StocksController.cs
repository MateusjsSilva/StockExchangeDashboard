using Microsoft.AspNetCore.Mvc;
using StocksEnchange.API.Services;

namespace StocksEnchange.API.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly StockService _stockService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(StockService stockService, ILogger<StocksController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var prices = _stockService.GetAllCurrentPrices();
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter preços das ações");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new { 
                message = "API de cotações em tempo real", 
                instructions = "Para receber atualizações em tempo real, conecte-se ao endpoint /stockHub usando SignalR"
            });
        }
    }
}