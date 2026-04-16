using FyersCSharpSDK;
using Newtonsoft.Json.Linq;
using RestSharp;
using Trading_UI.Components;
using HyperSyncLib;
using Newtonsoft.Json;

namespace Trading_UI.Services;

// Greeks data for options
public class Greeks
{
    [JsonProperty("Delta")]
    public decimal Delta { get; set; }

    [JsonProperty("Gamma")]
    public decimal Gamma { get; set; }

    [JsonProperty("Theta")]
    public decimal Theta { get; set; }

    [JsonProperty("Vega")]
    public decimal Vega { get; set; }

    [JsonProperty("Iv")]
    public decimal IV { get; set; }
}

// Raw option chain item from API
public class RawOptionChainItem
{
    [JsonProperty("Ask")]
    public decimal Ask { get; set; }

    [JsonProperty("Bid")]
    public decimal Bid { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Ex_symbol")]
    public string ExSymbol { get; set; }

    [JsonProperty("Exchange")]
    public string Exchange { get; set; }

    [JsonProperty("FyToken")]
    public string FyToken { get; set; }

    [JsonProperty("Ltp")]
    public decimal LTP { get; set; }

    [JsonProperty("Ltpch")]
    public decimal LTPChange { get; set; }

    [JsonProperty("Ltpchp")]
    public decimal LTPChangePercent { get; set; }

    [JsonProperty("Option_type")]
    public string OptionType { get; set; } // "CE", "PE", or empty for underlying

    [JsonProperty("Strike_price")]
    public decimal StrikePrice { get; set; }

    [JsonProperty("Symbol")]
    public string Symbol { get; set; }

    [JsonProperty("Oi")]
    public long OpenInterest { get; set; }

    [JsonProperty("Oich")]
    public long OIChange { get; set; }

    [JsonProperty("Oichp")]
    public decimal OIChangePercent { get; set; }

    [JsonProperty("Volume")]
    public long Volume { get; set; }

    [JsonProperty("Greeks")]
    public Greeks Greeks { get; set; }
}

// Expiry data
public class ExpiryData
{
    [JsonProperty("Date")]
    public string Date { get; set; }

    [JsonProperty("Expiry")]
    public string Expiry { get; set; }
}

// VIX data
public class VIXData
{
    [JsonProperty("Ask")]
    public decimal Ask { get; set; }

    [JsonProperty("Bid")]
    public decimal Bid { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Ltp")]
    public decimal LTP { get; set; }

    [JsonProperty("Ltpch")]
    public decimal LTPChange { get; set; }

    [JsonProperty("Ltpchp")]
    public decimal LTPChangePercent { get; set; }

    [JsonProperty("Symbol")]
    public string Symbol { get; set; }
}

// Raw API response
public class RawOptionChainResponse
{
    [JsonProperty("CallOi")]
    public string CallOI { get; set; }

    [JsonProperty("PutOi")]
    public string PutOI { get; set; }

    [JsonProperty("expiryData")]
    public List<ExpiryData> ExpiryData { get; set; } = new();

    [JsonProperty("indiavixData")]
    public List<VIXData> IndiaVixData { get; set; } = new();

    [JsonProperty("optionsChainData")]
    public List<RawOptionChainItem> OptionsChainData { get; set; } = new();
}

// UI-friendly option chain data (grouped by strike)
public class OptionChainData
{
    public double StrikePrice { get; set; }

    // Call option data
    public string CallSymbol { get; set; }
    public decimal CallBid { get; set; }
    public decimal CallAsk { get; set; }
    public decimal CallLTP { get; set; }
    public long CallOpenInterest { get; set; }
    public long CallVolume { get; set; }
    public decimal? CallIV { get; set; }
    public decimal? CallDelta { get; set; }
    public decimal? CallTheta { get; set; }
    public decimal? CallGamma { get; set; }
    public decimal? CallVega { get; set; }

    // Put option data
    public string PutSymbol { get; set; }
    public decimal PutBid { get; set; }
    public decimal PutAsk { get; set; }
    public decimal PutLTP { get; set; }
    public long PutOpenInterest { get; set; }
    public long PutVolume { get; set; }
    public decimal? PutIV { get; set; }
    public decimal? PutDelta { get; set; }
    public decimal? PutTheta { get; set; }
    public decimal? PutGamma { get; set; }
    public decimal? PutVega { get; set; }
}

public class OptionChainResponse
{
    public bool Success { get; set; }
    public List<OptionChainData> Data { get; set; } = new();
    public string ExpiryDate { get; set; }
    public string Message { get; set; }
    public List<ExpiryData> AvailableExpiries { get; set; } = new();
    public VIXData VIXData { get; set; }
    public long TotalCallOI { get; set; }
    public long TotalPutOI { get; set; }
}

public interface IOptionChainService
{
    Task<OptionChainResponse> GetOptionChainAsync(string symbol, string expiryDate);
    Task<List<string>> GetOptionExpiriesAsync(string symbol);
    Task<OptionChainResponse> SimulateOptionChainAsync(string symbol, decimal underlyingPrice);
}

public class OptionChainService : IOptionChainService
{
    private readonly HttpClient _httpClient;
    private readonly Random _random = new();

    public OptionChainService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<OptionChainResponse> GetOptionChainAsync(string symbol, string expiryDate)
    {
        try
        {
            string access_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOlsiZDoxIiwiZDoyIiwieDowIiwieDoxIiwieDoyIl0sImF0X2hhc2giOiJnQUFBQUFCcDM1SVRtNlRzNUdlSWJiYlFnZzlFYW9rcXIzNnpfdDNDZ1lLc0tNNEtZVXhnZDY5NGpidUFWUEwtQVRhc0FUblJXQTQ2UU5KWnZhTFZVTEpqaS1tYUxlV0wtODNrYUZQeUlUcTgtczV1OW1VdDhZOD0iLCJkaXNwbGF5X25hbWUiOiIiLCJvbXMiOiJLMSIsImhzbV9rZXkiOiJjNWM3NTRhNmEyNGEzMzY5NzNhNTEzZDQ1NDNkZDM3NDgxZWNjNWU4MDYwZTliZTY4M2FiNzg4MSIsImlzRGRwaUVuYWJsZWQiOiJOIiwiaXNNdGZFbmFibGVkIjoiTiIsImZ5X2lkIjoiRkFKMTA1MjkiLCJhcHBUeXBlIjoyMDAsImV4cCI6MTc3NjI5OTQwMCwiaWF0IjoxNzc2MjU5NjAzLCJpc3MiOiJhcGkuZnllcnMuaW4iLCJuYmYiOjE3NzYyNTk2MDMsInN1YiI6ImFjY2Vzc190b2tlbiJ9.QgvwWJUgCOX7olDiQy0dLwba29HAOtkDq_XZbEKgNWI";
            FyersClass stocks = FyersClass.Instance;
            stocks.ClientId = "26ZQB99CSZ-200";
            stocks.AccessToken = access_token;

            int strikeCount = 1;
            string timestamp = "";
            string greeks = "1";

            Tuple<JArray, JObject> stockTuple = await stocks.OptionsChain("NSE:NIFTY50-INDEX", strikeCount, timestamp, greeks);

            // Parse response at line 81
            if (stockTuple.Item1 != null)
            {
                return ParseOptionChainResponse(stockTuple.Item1, expiryDate);
            }
            else if (stockTuple.Item2 != null)
            {
                return ParseOptionChainResponse(stockTuple.Item2, expiryDate);
            }

            return new OptionChainResponse
            {
                Success = false,
                Message = "No data received from API"
            };
        }
        catch (Exception ex)
        {
            return new OptionChainResponse
            {
                Success = false,
                Message = $"Exception: {ex.Message}"
            };
        }
    }

    private OptionChainResponse ParseOptionChainResponse(JToken response, string expiryDate)
    {
        try
        {
            // If response is JArray, get first element
            if (response is JArray jArray)
            {
                if (jArray.Count == 0)
                    return new OptionChainResponse { Success = false, Message = "Empty response array" };

                response = jArray[0];
            }

            // Deserialize to our DTO
            var rawResponse = response.ToObject<RawOptionChainResponse>();

            if (rawResponse == null)
                return new OptionChainResponse { Success = false, Message = "Failed to deserialize response" };

            // Parse total OI values
            long.TryParse(rawResponse.CallOI, out long callOI);
            long.TryParse(rawResponse.PutOI, out long putOI);

            // Group options by strike price (CE and PE pairs)
            var optionsByStrike = rawResponse.OptionsChainData
                .Where(x => !string.IsNullOrEmpty(x.OptionType)) // Exclude underlying price rows
                .GroupBy(x => x.StrikePrice)
                .OrderBy(g => g.Key)
                .ToList();

            var chainData = new List<OptionChainData>();

            foreach (var strikeGroup in optionsByStrike)
            {
                var chainItem = new OptionChainData
                {
                    StrikePrice = (double)strikeGroup.Key
                };

                var callOption = strikeGroup.FirstOrDefault(x => x.OptionType == "CE");
                if (callOption != null)
                {
                    chainItem.CallSymbol = callOption.Symbol;
                    chainItem.CallBid = callOption.Bid;
                    chainItem.CallAsk = callOption.Ask;
                    chainItem.CallLTP = callOption.LTP;
                    chainItem.CallOpenInterest = callOption.OpenInterest;
                    chainItem.CallVolume = callOption.Volume;

                    if (callOption.Greeks != null)
                    {
                        chainItem.CallDelta = callOption.Greeks.Delta;
                        chainItem.CallGamma = callOption.Greeks.Gamma;
                        chainItem.CallTheta = callOption.Greeks.Theta;
                        chainItem.CallVega = callOption.Greeks.Vega;
                        chainItem.CallIV = callOption.Greeks.IV;
                    }
                }

                var putOption = strikeGroup.FirstOrDefault(x => x.OptionType == "PE");
                if (putOption != null)
                {
                    chainItem.PutSymbol = putOption.Symbol;
                    chainItem.PutBid = putOption.Bid;
                    chainItem.PutAsk = putOption.Ask;
                    chainItem.PutLTP = putOption.LTP;
                    chainItem.PutOpenInterest = putOption.OpenInterest;
                    chainItem.PutVolume = putOption.Volume;

                    if (putOption.Greeks != null)
                    {
                        chainItem.PutDelta = putOption.Greeks.Delta;
                        chainItem.PutGamma = putOption.Greeks.Gamma;
                        chainItem.PutTheta = putOption.Greeks.Theta;
                        chainItem.PutVega = putOption.Greeks.Vega;
                        chainItem.PutIV = putOption.Greeks.IV;
                    }
                }

                chainData.Add(chainItem);
            }

            // Get VIX data
            var vixData = rawResponse.IndiaVixData?.FirstOrDefault();

            return new OptionChainResponse
            {
                Success = true,
                Data = chainData,
                ExpiryDate = expiryDate,
                AvailableExpiries = rawResponse.ExpiryData,
                VIXData = vixData,
                TotalCallOI = callOI,
                TotalPutOI = putOI,
                Message = "Option chain data parsed successfully"
            };
        }
        catch (Exception ex)
        {
            return new OptionChainResponse
            {
                Success = false,
                Message = $"Parsing error: {ex.Message}"
            };
        }
    }

    public async Task<List<string>> GetOptionExpiriesAsync(string symbol)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/trading/option-expiries/{symbol}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<string>>(content) ?? new();
            }

            return new();
        }
        catch
        {
            return new();
        }
    }

    public Task<OptionChainResponse> SimulateOptionChainAsync(string symbol, decimal underlyingPrice)
    {
        var data = new List<OptionChainData>();

        var strikes = new List<decimal>();
        for (decimal strike = underlyingPrice - 500; strike <= underlyingPrice + 500; strike += 50)
        {
            strikes.Add(strike);
        }

        foreach (var strike in strikes)
        {
            var callPrice = Math.Max(underlyingPrice - strike, 0) + (decimal)(_random.NextDouble() * 50);
            var putPrice = Math.Max(strike - underlyingPrice, 0) + (decimal)(_random.NextDouble() * 50);

            data.Add(new OptionChainData
            {
                StrikePrice = (double)strike,
                CallSymbol = $"{symbol}C{strike:F0}",
                CallBid = callPrice * 0.98m,
                CallAsk = callPrice * 1.02m,
                CallLTP = callPrice,
                CallIV = (decimal)(_random.NextDouble() * 50 + 15),
                CallDelta = (decimal)(_random.NextDouble() * 0.8 + 0.1),
                CallTheta = (decimal)(_random.NextDouble() * -0.5),
                CallVolume = _random.Next(100, 5000),
                CallOpenInterest = _random.Next(10000, 500000),

                PutSymbol = $"{symbol}P{strike:F0}",
                PutBid = putPrice * 0.98m,
                PutAsk = putPrice * 1.02m,
                PutLTP = putPrice,
                PutIV = (decimal)(_random.NextDouble() * 50 + 15),
                PutDelta = (decimal)(_random.NextDouble() * -0.8 - 0.1),
                PutTheta = (decimal)(_random.NextDouble() * -0.5),
                PutVolume = _random.Next(100, 5000),
                PutOpenInterest = _random.Next(10000, 500000)
            });
        }

        return Task.FromResult(new OptionChainResponse
        {
            Success = true,
            Data = data,
            ExpiryDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd")
        });
    }
}
