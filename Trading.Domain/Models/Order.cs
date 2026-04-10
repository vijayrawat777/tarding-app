using System;

namespace Trading.Domain.Models
{
    public enum OrderSide
    {
        Buy = 1,
        Sell = 2
    }

    public enum OrderStatus
    {
        Pending = 1,
        Accepted = 2,
        Executed = 3,
        PartiallyExecuted = 4,
        Cancelled = 5,
        Rejected = 6
    }

    public enum OrderType
    {
        Market = 1,
        Limit = 2,
        Stop = 3,
        StopLimit = 4
    }

    public class Order
    {
        public string Id { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; }
        public bool IsOptionTrade { get; set; } // true for option, false for stock
        
        public OrderSide Side { get; set; }
        public OrderType Type { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? TriggeredPrice { get; set; }
        
        public OrderStatus Status { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public decimal ExecutedPrice { get; set; }
        
        public string StrategyName { get; set; }
        public string StrategyType { get; set; } // EMA, RSI, Manual, etc.
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public string Notes { get; set; }

        public Order()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            ExecutedQuantity = 0;
        }

        public void Execute(decimal executedPrice, decimal executedQty)
        {
            ExecutedPrice = executedPrice;
            ExecutedQuantity = executedQty;
            ExecutedAt = DateTime.UtcNow;
            Status = executedQty == Quantity ? OrderStatus.Executed : OrderStatus.PartiallyExecuted;
        }

        public void Cancel()
        {
            Status = OrderStatus.Cancelled;
        }
    }
}
