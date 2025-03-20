namespace StocksEnchange.API.Models
{
    /// <summary>
    /// Represents the price information of a stock.
    /// </summary>
    public class StockPrice
    {
        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        public string? Symbol { get; set; }

        /// <summary>
        /// Gets or sets the stock price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the stock price.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the change in stock price.
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// Gets or sets the percentage change in stock price.
        /// </summary>
        public decimal PercentChange { get; set; }
    }
}