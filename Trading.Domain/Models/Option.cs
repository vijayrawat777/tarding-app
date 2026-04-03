using System;

namespace Trading.Domain.Models
{
    public class Option
    {
        public string Symbol { get; private set; }
        public string UnderlyingSymbol { get; private set; }
        public OptionType Type { get; private set; }
        public decimal StrikePrice { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        
        // Pricing
        public decimal LastTradedPrice { get; private set; }
        public decimal Change { get; private set; }
        public decimal ChangePercent { get; private set; }
        
        // Bid/Ask
        public decimal BidPrice { get; private set; }
        public int BidQuantity { get; private set; }
        public decimal AskPrice { get; private set; }
        public int AskQuantity { get; private set; }
        
        // Volume & OI
        public long Volume { get; private set; }
        public long OpenInterest { get; private set; }
        public long OpenInterestChange { get; private set; }
        
        // Greeks (optional)
        public decimal? ImpliedVolatility { get; private set; }
        public decimal? Delta { get; private set; }
        public decimal? Gamma { get; private set; }
        public decimal? Theta { get; private set; }
        public decimal? Vega { get; private set; }
        
        public DateTime LastUpdated { get; private set; }
        public bool IsActive { get; private set; }

        private Option() { } // EF Core constructor

        public Option(string symbol, string underlyingSymbol, OptionType type, decimal strikePrice, DateTime expiryDate)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            UnderlyingSymbol = underlyingSymbol ?? throw new ArgumentNullException(nameof(underlyingSymbol));
            Type = type;
            StrikePrice = strikePrice;
            ExpiryDate = expiryDate;
            IsActive = true;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdatePricing(decimal ltp, decimal change, decimal changePercent)
        {
            LastTradedPrice = ltp;
            Change = change;
            ChangePercent = changePercent;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateBidAsk(decimal bidPrice, int bidQty, decimal askPrice, int askQty)
        {
            BidPrice = bidPrice;
            BidQuantity = bidQty;
            AskPrice = askPrice;
            AskQuantity = askQty;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateVolumeOI(long volume, long oi, long oiChange)
        {
            Volume = volume;
            OpenInterest = oi;
            OpenInterestChange = oiChange;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateGreeks(decimal? iv, decimal? delta, decimal? gamma, decimal? theta, decimal? vega)
        {
            ImpliedVolatility = iv;
            Delta = delta;
            Gamma = gamma;
            Theta = theta;
            Vega = vega;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public enum OptionType
    {
        Call,
        Put
    }
}
