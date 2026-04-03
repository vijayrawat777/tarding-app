namespace Trading.Application.DTOs
{
    public class OptionChainDto
    {
        public string UnderlyingSymbol { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public decimal UnderlyingPrice { get; set; }
        public decimal UnderlyingChange { get; set; }
        public decimal UnderlyingChangePercent { get; set; }

        public List<OptionDto> Calls { get; set; } = new();
        public List<OptionDto> Puts { get; set; } = new();

        public DateTime LastUpdated { get; set; }
    }
}
