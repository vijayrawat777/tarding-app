namespace Trading.Application.DTOs
{
    public class TradeDto
    {
        public string Id { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; }
        public bool IsOptionTrade { get; set; }
        
        public string EntryDirection { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal Quantity { get; set; }
        public System.DateTime EntryTime { get; set; }
        
        public decimal? ExitPrice { get; set; }
        public System.DateTime? ExitTime { get; set; }
        public string Status { get; set; }
        
        public string StrategyName { get; set; }
        public string StrategyType { get; set; }
        
        public decimal GrossProfit { get; set; }
        public decimal? NetProfit { get; set; }
        public decimal? ProfitPercent { get; set; }
        
        public decimal? StopLossPrice { get; set; }
        public decimal? TakeProfitPrice { get; set; }
        public decimal? MaxDrawdown { get; set; }
        
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? ClosedAt { get; set; }
    }
}
