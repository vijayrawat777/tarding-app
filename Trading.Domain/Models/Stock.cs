using System;

namespace Trading.Domain.Models
{
    public class Stock
    {
        public string Symbol { get; private set; }
        public string Name { get; private set; }
        public string Exchange { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public decimal PreviousClose { get; private set; }
        public decimal OpenPrice { get; private set; }
        public decimal DayHigh { get; private set; }
        public decimal DayLow { get; private set; }
        public long Volume { get; private set; }
        public decimal MarketCap { get; private set; }
        public decimal Change { get; private set; }
        public decimal ChangePercent { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public bool IsActive { get; private set; }

        private Stock() { } // EF Core constructor

        public Stock(string symbol, string name, string exchange)
        {
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            IsActive = true;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal currentPrice, decimal previousClose, decimal openPrice, 
                               decimal dayHigh, decimal dayLow, long volume)
        {
            CurrentPrice = currentPrice;
            PreviousClose = previousClose;
            OpenPrice = openPrice;
            DayHigh = dayHigh;
            DayLow = dayLow;
            Volume = volume;
            Change = CurrentPrice - PreviousClose;
            ChangePercent = PreviousClose != 0 ? (Change / PreviousClose) * 100 : 0;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateMarketCap(decimal marketCap)
        {
            MarketCap = marketCap;
            LastUpdated = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
