namespace Trading.Application.DTOs
{
    public class TradeLogDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string TradeId { get; set; }
        public string OrderId { get; set; }
        public string StrategyName { get; set; }
        
        public string Symbol { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PnL { get; set; }
        public string Details { get; set; }
        
        public string Severity { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
