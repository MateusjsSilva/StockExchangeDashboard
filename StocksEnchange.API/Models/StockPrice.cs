namespace StocksEnchange.API.Models
{
    public class StockPrice
    {
        public string Symbol { get; set; }

        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Change { get; set; }

        public decimal PercentChange { get; set; }
    }
}