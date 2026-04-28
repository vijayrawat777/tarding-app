using FyersCSharpSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Domain.Models.Swing;

namespace Trading.Infrastructure.Services.Swing
{
    public interface IFyersService
    {
        Task<List<Candle>> GetHistoricalData(string accessToken, StockHistoryModel stockModel);
    }
}
