namespace Trading_UI.Services;

public interface IFyersDataService
{
    Task<StockQuoteResponse> GetStockQuoteAsync(string symbol);
    Task<IEnumerable<StockQuoteResponse>> GetStockQuotesAsync(IEnumerable<string> symbols);
    Task<bool> IsMarketOpenAsync();
}

public class StockQuoteResponse
{
    public bool Success { get; set; }
    public StockData Data { get; set; }
    public string Message { get; set; }
}

public class StockData
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Exchange { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal PreviousClose { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal DayHigh { get; set; }
    public decimal DayLow { get; set; }
    public long Volume { get; set; }
    public decimal MarketCap { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; }
}

public class MarketStatusResponse
{
    public bool Success { get; set; }
    public bool MarketOpen { get; set; }
}
