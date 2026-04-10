namespace Trading.Application.DTOs
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; }
        public bool IsOptionTrade { get; set; }
        
        public string Side { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public decimal ExecutedPrice { get; set; }
        
        public string StrategyName { get; set; }
        public string StrategyType { get; set; }
        
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? ExecutedAt { get; set; }
        public string Notes { get; set; }
    }
}
