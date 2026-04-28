using FyersCSharpSDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Domain.Models.Swing;
using Trading.Infrastructure.Configuration;

namespace Trading.Infrastructure.Services.Swing
{
    public class FyersService : IFyersService
    {
        private readonly FyersConfig _fyersConfig;
        public FyersService(FyersConfig fyersConfig)
        {
            _fyersConfig = fyersConfig;
        }

        public async Task<List<Candle>> GetHistoricalData(string accessToken, StockHistoryModel stockModel)
        {
            FyersClass stocks = FyersClass.Instance;
            stocks.ClientId = _fyersConfig.AppId;
            stocks.AccessToken = accessToken;

            Tuple<JArray, JObject> stockTuple = await stocks.GetStockHistory(stockModel);

            return stockTuple.Item1
                .Select(c => new Candle
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds((long)c[0]).DateTime,
                    Open = (decimal)c[1],
                    High = (decimal)c[2],
                    Low = (decimal)c[3],
                    Close = (decimal)c[4],
                    Volume = (long)c[5]
                }).ToList();
        }

    }
}
