public class TradeSignal
{
    public string Signal { get; set; }
    public decimal Strike { get; set; }
    public string Symbol { get; set; }
    public decimal Confidence { get; set; }
    public decimal Entry { get; set; }
    public decimal StopLoss { get; set; }
    public decimal Target { get; set; }
    public string Reason { get; set; }
}