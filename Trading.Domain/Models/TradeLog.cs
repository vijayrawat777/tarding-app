using System;

namespace Trading.Domain.Models
{
    public enum TradeLogType
    {
        OrderPlaced = 1,
        OrderExecuted = 2,
        OrderCancelled = 3,
        TradeOpened = 4,
        TradeClosed = 5,
        StopLossTriggered = 6,
        TakeProfitTriggered = 7,
        StrategySignal = 8,
        StrategyError = 9,
        AutoTradingStarted = 10,
        AutoTradingStopped = 11
    }

    public class TradeLog
    {
        public string Id { get; set; }
        public TradeLogType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        // Relation
        public string TradeId { get; set; }
        public string OrderId { get; set; }
        public string StrategyName { get; set; }
        
        // Details
        public string Symbol { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PnL { get; set; }
        public string Details { get; set; } // JSON for complex data
        
        // Severity
        public string Severity { get; set; } = "Info"; // Info, Warning, Error
        
        public DateTime CreatedAt { get; set; }

        public TradeLog()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public static TradeLog CreateOrderLog(Order order, string title, string description)
        {
            return new TradeLog
            {
                OrderId = order.Id,
                Symbol = order.UnderlyingSymbol,
                Type = TradeLogType.OrderPlaced,
                Title = title,
                Description = description,
                Price = order.Price,
                Quantity = order.Quantity,
                StrategyName = order.StrategyName
            };
        }

        public static TradeLog CreateTradeLog(Trade trade, string title, string description)
        {
            return new TradeLog
            {
                TradeId = trade.Id,
                Symbol = trade.UnderlyingSymbol,
                Type = TradeLogType.TradeOpened,
                Title = title,
                Description = description,
                Price = trade.EntryPrice,
                Quantity = trade.Quantity,
                StrategyName = trade.StrategyName
            };
        }
    }
}
