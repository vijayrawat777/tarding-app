namespace Trading.Application.DTOs
{
    public class OptionDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string UnderlyingSymbol { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Call" or "Put"
        public decimal StrikePrice { get; set; }
        public DateTime ExpiryDate { get; set; }

        // Pricing
        public decimal LastTradedPrice { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }

        // Bid/Ask
        public decimal BidPrice { get; set; }
        public int BidQuantity { get; set; }
        public decimal AskPrice { get; set; }
        public int AskQuantity { get; set; }

        // Volume & OI
        public long Volume { get; set; }
        public long OpenInterest { get; set; }
        public long OpenInterestChange { get; set; }

        // Greeks
        public decimal? ImpliedVolatility { get; set; }
        public decimal? Delta { get; set; }
        public decimal? Gamma { get; set; }
        public decimal? Theta { get; set; }
        public decimal? Vega { get; set; }

        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; }
    }
}
