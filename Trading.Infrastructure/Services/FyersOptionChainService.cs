using FyersCSharpSDK;
using HyperSyncLib;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
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

        private OptionChainResponse ParseOptionChainResponse(JToken response, string expiryDate)
        {
            try
            {
                var raw = response;

                var optionsArray = raw["optionsChainData"] as JArray;
                var expiryArray = raw["expiryData"]?.ToObject<List<ExpiryData>>();
                var vix = raw["indiavixData"]?.FirstOrDefault();

                if (optionsArray == null || !optionsArray.Any())
                {
                    return new OptionChainResponse
                    {
                        Success = false,
                        Message = "No option chain data found"
                    };
                }

                // Extract total OI safely
                long callOI = raw["CallOi"]?.Value<long>() ?? 0;
                long putOI = raw["PutOi"]?.Value<long>() ?? 0;

                // Filter CE/PE only
                var optionRows = optionsArray
                    .Where(x => !string.IsNullOrEmpty(x["Option_type"]?.ToString()))
                    .ToList();

                // Group by strike
                var grouped = optionRows
                    .GroupBy(x => x["Strike_price"]?.Value<decimal>())
                    .OrderBy(x => x.Key);

                var chainData = new List<OptionChainData>();

                foreach (var group in grouped)
                {
                    var item = new OptionChainData
                    {
                        StrikePrice = (double)group.Key
                    };

                    foreach (var opt in group)
                    {
                        var type = opt["Option_type"]?.ToString();

                        var data = new
                        {
                            Symbol = opt["Symbol"]?.ToString(),
                            Bid = opt["Bid"]?.Value<decimal>() ?? 0,
                            Ask = opt["Ask"]?.Value<decimal>() ?? 0,
                            LTP = opt["Ltp"]?.Value<decimal>() ?? 0,
                            OI = opt["Oi"]?.Value<long>() ?? 0,
                            Volume = opt["Volume"]?.Value<long>() ?? 0
                        };

                        if (type == "CE")
                        {
                            item.CallSymbol = data.Symbol;
                            item.CallBid = data.Bid;
                            item.CallAsk = data.Ask;
                            item.CallLTP = data.LTP;
                            item.CallOpenInterest = data.OI;
                            item.CallVolume = data.Volume;
                        }
                        else if (type == "PE")
                        {
                            item.PutSymbol = data.Symbol;
                            item.PutBid = data.Bid;
                            item.PutAsk = data.Ask;
                            item.PutLTP = data.LTP;
                            item.PutOpenInterest = data.OI;
                            item.PutVolume = data.Volume;
                        }
                    }

                    chainData.Add(item);
                }


                return new OptionChainResponse
                {
                    Success = true,
                    Data = chainData,
                    ExpiryDate = expiryDate,
                    AvailableExpiries = expiryArray,
                    VIXData = vix?.ToObject<VixData>(),
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

       
    }
}

