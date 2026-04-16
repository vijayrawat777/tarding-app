namespace Trading_UI.Services;

public class HistoricalDataPoint
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
}

public class HistoricalDataResponse
{
    public bool Success { get; set; }
    public List<HistoricalDataPoint> Data { get; set; } = new();
    public string Message { get; set; }
}

public interface IHistoricalDataService
{
    Task<HistoricalDataResponse> GetHistoricalDataAsync(string symbol, DateTime fromDate, DateTime toDate);
    Task<List<HistoricalDataPoint>> SimulateHistoricalDataAsync(string symbol, int days = 30);
}

public class HistoricalDataService : IHistoricalDataService
{
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();

    public HistoricalDataService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<HistoricalDataResponse> GetHistoricalDataAsync(string symbol, DateTime fromDate, DateTime toDate)
    {
        try
        {
            // Call the real Trading.API endpoint which calls Fyers API
            var response = await _httpClient.GetAsync(
                $"/api/trading/historical-data/{symbol}?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}"
            );
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<HistoricalDataResponse>(content);
                return result ?? new HistoricalDataResponse { Success = false, Message = "Failed to parse response" };
            }

            return new HistoricalDataResponse 
            { 
                Success = false, 
                Message = $"API Error: {response.StatusCode} - {response.ReasonPhrase}" 
            };
        }
        catch (Exception ex)
        {
            return new HistoricalDataResponse 
            { 
                Success = false, 
                Message = $"Exception: {ex.Message}" 
            };
        }
    }

    public async Task<List<HistoricalDataPoint>> SimulateHistoricalDataAsync(string symbol, int days = 30)
    {
        var data = new List<HistoricalDataPoint>();
        decimal currentPrice = 100m;
        
        for (int i = days; i > 0; i--)
        {
            var date = DateTime.Now.AddDays(-i);
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                continue;

            var randomDouble = _random.NextDouble();
            var changePercent = (decimal)(randomDouble - 0.5) * 5;
            var open = currentPrice;
            var high = currentPrice + Math.Abs(changePercent) + (decimal)(_random.NextDouble() * 2);
            var low = currentPrice - Math.Abs(changePercent) - (decimal)(_random.NextDouble() * 2);
            var close = low + (decimal)(_random.NextDouble() * (double)(high - low));

            data.Add(new HistoricalDataPoint
            {
                Date = date,
                Open = open,
                High = high,
                Low = low,
                Close = close,
                Volume = (long)(_random.Next(1000000, 10000000))
            });

            currentPrice = close;
        }

        return data;
    }
}
