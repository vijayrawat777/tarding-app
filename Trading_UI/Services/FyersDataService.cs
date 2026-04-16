using System.Net.Http.Json;

namespace Trading_UI.Services;

public class FyersDataService : IFyersDataService
{
    private readonly HttpClient _httpClient;

    public FyersDataService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<StockQuoteResponse> GetStockQuoteAsync(string symbol)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/trading/stock-details/{symbol}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<StockQuoteResponse>(content) 
                    ?? new StockQuoteResponse { Success = false };
            }

            return new StockQuoteResponse 
            { 
                Success = false, 
                Message = $"Error: {response.StatusCode}" 
            };
        }
        catch (Exception ex)
        {
            return new StockQuoteResponse 
            { 
                Success = false, 
                Message = $"Exception: {ex.Message}" 
            };
        }
    }

    public async Task<IEnumerable<StockQuoteResponse>> GetStockQuotesAsync(IEnumerable<string> symbols)
    {
        var tasks = symbols.Select(s => GetStockQuoteAsync(s)).ToList();
        var results = await Task.WhenAll(tasks);
        return results;
    }

    public async Task<bool> IsMarketOpenAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/trading/market-status");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var json = System.Text.Json.JsonDocument.Parse(content);
                return json.RootElement.TryGetProperty("marketOpen", out var val) && val.GetBoolean();
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
