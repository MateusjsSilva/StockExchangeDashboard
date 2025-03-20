using Microsoft.AspNetCore.Mvc;
using StocksEnchange.API.Services;

namespace StocksEnchange.API.Controllers
{
    [Route("api/stock")]
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
                _logger.LogError(ex, "Error getting stock prices");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new { 
                message = "Real-time stock quotes API", 
                instructions = "To receive real-time updates, connect to the /stockHub endpoint using SignalR"
            });
        }
    }
}