using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Application.DTOs.OptionChain
{
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

}
