using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Application.DTOs.OptionChain
{

    public class OptionChainResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public List<OptionChainData> Data { get; set; } = new();

        public string ExpiryDate { get; set; }

        public List<ExpiryData> AvailableExpiries { get; set; } = new();

        public VixData VIXData { get; set; }

        public long TotalCallOI { get; set; }
        public long TotalPutOI { get; set; }

        public decimal ATM { get; set; }
        public long Support { get; set; }
        public long Resistance { get; set; }
    }

    public class ExpiryData
    {
        public string Date { get; set; }
        public string Expiry { get; set; }
    }

    public class RawOption
    {
        [JsonProperty("Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("Strike_price")]
        public decimal StrikePrice { get; set; }

        [JsonProperty("Option_type")]
        public string OptionType { get; set; }

        [JsonProperty("Ltp")]
        public decimal LTP { get; set; }

        [JsonProperty("Bid")]
        public decimal Bid { get; set; }

        [JsonProperty("Ask")]
        public decimal Ask { get; set; }

        [JsonProperty("Oi")]
        public long OpenInterest { get; set; }

        [JsonProperty("Volume")]
        public long Volume { get; set; }
    }
    public class VixData
    {
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public string Description { get; set; }
        public string Ex_symbol { get; set; }
        public string Exchange { get; set; }
        public string FyToken { get; set; }

        public decimal Ltp { get; set; }
        public decimal Ltpch { get; set; }
        public decimal Ltpchp { get; set; }

        public string Symbol { get; set; }
    }
    public class OptionChainData
    {
        public decimal StrikePrice { get; set; }

        public string CallSymbol { get; set; }
        public decimal CallBid { get; set; }
        public decimal CallAsk { get; set; }
        public decimal CallLTP { get; set; }
        public long CallOpenInterest { get; set; }
        public long CallVolume { get; set; }
        public Greeks CallGreeks { get; set; }

        public string PutSymbol { get; set; }
        public decimal PutBid { get; set; }
        public decimal PutAsk { get; set; }
        public decimal PutLTP { get; set; }
        public long PutOpenInterest { get; set; }
        public long PutVolume { get; set; }
        public Greeks PutGreeks { get; set; }

        // 🔥 Greeks (VERY IMPORTANT for trading engine)
        public decimal CallDelta { get; set; }
        public decimal CallGamma { get; set; }
        public decimal CallTheta { get; set; }
        public decimal CallVega { get; set; }
        public decimal CallIV { get; set; }

        public decimal PutDelta { get; set; }
        public decimal PutGamma { get; set; }
        public decimal PutTheta { get; set; }
        public decimal PutVega { get; set; }
        public decimal PutIV { get; set; }
    }
}
