using System;

namespace Trading.Domain.Models
{
    public enum TradeStatus
    {
        Open = 1,
        Closed = 2,
        StopLoss = 3,
        TakeProfit = 4,
        Manual = 5
    }

    public class Trade
    {
        public string Id { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; }
        public bool IsOptionTrade { get; set; }
        
        // Entry
        public OrderSide EntryDirection { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal Quantity { get; set; }
        public DateTime EntryTime { get; set; }
        public string EntryOrderId { get; set; }
        
        // Exit
        public decimal? ExitPrice { get; set; }
        public DateTime? ExitTime { get; set; }
        public string ExitOrderId { get; set; }
        public TradeStatus Status { get; set; }
        
        // Strategy & PnL
        public string StrategyName { get; set; }
        public string StrategyType { get; set; }
        
        public decimal GrossProfit { get; set; }
        public decimal? NetProfit { get; set; }
        public decimal? ProfitPercent { get; set; }
        
        // Risk Management
        public decimal? StopLossPrice { get; set; }
        public decimal? TakeProfitPrice { get; set; }
        public decimal? MaxDrawdown { get; set; }
        
        // Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Notes { get; set; }

        public Trade()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            Status = TradeStatus.Open;
            GrossProfit = 0;
        }

        public void Close(decimal exitPrice)
        {
            ExitPrice = exitPrice;
            ExitTime = DateTime.UtcNow;
            ClosedAt = DateTime.UtcNow;
            Status = TradeStatus.Closed;
            
            CalculatePnL();
        }

        public void CalculatePnL()
        {
            if (ExitPrice.HasValue)
            {
                decimal priceDiff = (ExitPrice.Value - EntryPrice) * Quantity;
                
                if (EntryDirection == OrderSide.Sell)
                    GrossProfit = -priceDiff;
                else
                    GrossProfit = priceDiff;

                ProfitPercent = (GrossProfit / (EntryPrice * Quantity)) * 100;
                NetProfit = GrossProfit; // Assuming no commission
            }
        }

        public bool IsInProfit() => GrossProfit > 0;
    }
}
