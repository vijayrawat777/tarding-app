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
                FyersClass stocks = FyersClass.Instance;
                stocks.ClientId = _fyersConfig.AppId;
                stocks.AccessToken = requestDto.AccessToken;

                Tuple<JArray, JObject> stockTuple = await stocks.OptionsChain(requestDto.Symbol, requestDto.StrikeCount, requestDto.Timestamp, requestDto.Greek);
                // Parse response at line 81
                if (stockTuple.Item1 != null)
                {
                    return ParseOptionChainResponse(stockTuple.Item1, requestDto.ExpiryDate);
                }
                else if (stockTuple.Item2 != null)
                {
                    return ParseOptionChainResponse(stockTuple.Item2, requestDto.ExpiryDate);
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
    }
}

