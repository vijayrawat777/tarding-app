public class TradeSignal
{
    public string Signal { get; set; }
    public double Strike { get; set; }
    public string Symbol { get; set; }
    public double Confidence { get; set; }
    public decimal Entry { get; set; }
    public decimal StopLoss { get; set; }
    public decimal Target { get; set; }
    public string Reason { get; set; }
}