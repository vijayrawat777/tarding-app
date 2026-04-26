using FyersCSharpSDK;
using Newtonsoft.Json.Linq;
using Trading.Application.DTOs.OptionChain;
using Trading.Infrastructure.Configuration;
using ExpiryData = Trading.Application.DTOs.OptionChain.ExpiryData;

namespace Trading.Infrastructure.Services
{
    /// <summary>
    /// Fyers option chain service for fetching option chain data
    /// </summary>
    public interface IFyersOptionChainService
    {
        Task<OptionChainResponse> GetOptionChainData(OptionChainRequestDto requestDto);
        Task<decimal> GetOptionChainSpotPrice(OptionChainRequestDto requestDto);

    }

    public class FyersOptionChainService : IFyersOptionChainService
    {
        private readonly FyersConfig _fyersConfig;

        public FyersOptionChainService(
            FyersConfig fyersConfig)
        {
            _fyersConfig = fyersConfig;
        }

        public async Task<decimal> GetOptionChainSpotPrice(OptionChainRequestDto requestDto)
        {
            FyersClass stocks = FyersClass.Instance;
            stocks.ClientId = _fyersConfig.AppId;
            stocks.AccessToken = requestDto.AccessToken;
            var result = await stocks.GetStockQuotes(requestDto.Symbol);

            List<StockModel> stocksList = result.Item1;

            //// ✔ Raw JSON (if needed)
            JObject rawJson = result.Item2;
            if (stocksList != null && stocksList.Count > 0)
            {
                return (decimal)stocksList[0].LimitPrice;
            }
            return (decimal)0.0;

        }

        public async Task<OptionChainResponse> GetOptionChainData(OptionChainRequestDto requestDto)
        {
            try
            {
                var stocks = FyersClass.Instance;
                stocks.ClientId = _fyersConfig.AppId;
                stocks.AccessToken = requestDto.AccessToken;

                var stockTuple = await stocks.OptionsChain(
                    requestDto.Symbol,
                    requestDto.StrikeCount,
                    requestDto.Timestamp,
                    requestDto.Greek);

                // Normalize response (VERY IMPORTANT)
                JToken responseToken = stockTuple.Item1?.FirstOrDefault()
                                       ?? stockTuple.Item2;

                if (responseToken == null)
                {
                    return new OptionChainResponse
                    {
                        Success = false,
                        Message = "No data received from API"
                    };
                }

                return ParseOptionChainResponse(responseToken, requestDto.ExpiryDate);
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

        public OptionChainResponse ParseOptionChainResponse(JToken raw, string expiryDate)
        {
            try
            {
                // ✅ Extract arrays safely
                var optionsArray = raw["optionsChainData"] as JArray;
                var expiryArray = raw["expiryData"]?.ToObject<List<ExpiryData>>();

                // VIX can be array OR object
                var vixToken = raw["indiavixData"] is JArray arr
                    ? arr.FirstOrDefault()
                    : raw["indiavixData"];

                if (optionsArray == null || !optionsArray.Any())
                {
                    return new OptionChainResponse
                    {
                        Success = false,
                        Message = "No option chain data found"
                    };
                }

                // ✅ OI safe parse (string/number both supported)
                long callOI = GetLong(raw["CallOi"]);
                long putOI = GetLong(raw["PutOi"]);

                // ✅ Filter valid options only
                var optionRows = optionsArray
                    .Where(x =>
                        !string.IsNullOrEmpty(x["Option_type"]?.ToString()) &&
                        (x["Option_type"].ToString() == "CE" || x["Option_type"].ToString() == "PE"))
                    .ToList();

                // ✅ Group by strike
                var grouped = optionRows
                    .GroupBy(x => x["Strike_price"]?.Value<decimal>() ?? 0)
                    .Where(g => g.Key > 0)
                    .OrderBy(g => g.Key);

                var chainData = new List<OptionChainData>();

                foreach (var group in grouped)
                {
                    var item = new OptionChainData
                    {
                        StrikePrice = group.Key
                    };

                    foreach (var opt in group)
                    {
                        var type = opt["Option_type"]?.ToString();

                        // ✅ Handle Greeks (case-insensitive)
                        var greeks = opt["Greeks"] ?? opt["greeks"];

                        decimal GetDecimal(JToken t, string name) =>
                            t?[name]?.Value<decimal?>()
                            ?? t?[name.ToLower()]?.Value<decimal?>()
                            ?? 0;

                        var delta = GetDecimal(greeks, "Delta");
                        var gamma = GetDecimal(greeks, "Gamma");
                        var theta = GetDecimal(greeks, "Theta");
                        var vega = GetDecimal(greeks, "Vega");
                        var iv = GetDecimal(greeks, "Iv");

                        // ✅ Common data
                        var symbol = opt["Symbol"]?.ToString();
                        var bid = opt["Bid"]?.Value<decimal>() ?? 0;
                        var ask = opt["Ask"]?.Value<decimal>() ?? 0;
                        var ltp = opt["Ltp"]?.Value<decimal>() ?? 0;
                        var oi = opt["Oi"]?.Value<long>() ?? 0;
                        var volume = opt["Volume"]?.Value<long>() ?? 0;

                        if (type == "CE")
                        {
                            item.CallSymbol = symbol;
                            item.CallBid = bid;
                            item.CallAsk = ask;
                            item.CallLTP = ltp;
                            item.CallOpenInterest = oi;
                            item.CallVolume = volume;

                            item.CallDelta = delta;
                            item.CallGamma = gamma;
                            item.CallTheta = theta;
                            item.CallVega = vega;
                            item.CallIV = iv;
                        }
                        else if (type == "PE")
                        {
                            item.PutSymbol = symbol;
                            item.PutBid = bid;
                            item.PutAsk = ask;
                            item.PutLTP = ltp;
                            item.PutOpenInterest = oi;
                            item.PutVolume = volume;

                            item.PutDelta = delta;
                            item.PutGamma = gamma;
                            item.PutTheta = theta;
                            item.PutVega = vega;
                            item.PutIV = iv;
                        }
                    }

                    // ✅ Add only valid strikes (at least one side exists)
                    if (!string.IsNullOrEmpty(item.CallSymbol) || !string.IsNullOrEmpty(item.PutSymbol))
                    {
                        chainData.Add(item);
                    }
                }

                return new OptionChainResponse
                {
                    Success = true,
                    Data = chainData,
                    ExpiryDate = expiryDate,
                    AvailableExpiries = expiryArray,
                    VIXData = vixToken?.ToObject<VixData>(),
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

        private long GetLong(JToken token)
        {
            if (token == null) return 0;

            if (long.TryParse(token.ToString(), out var val))
                return val;

            return token.Value<long?>() ?? 0;
        }
               
    }
}

