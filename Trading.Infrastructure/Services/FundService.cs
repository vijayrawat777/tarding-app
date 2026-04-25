using FyersCSharpSDK;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Application.DTOs;
using Trading.Infrastructure.Configuration;

namespace Trading.Infrastructure.Services
{
    public interface IFundProfileService
    {
        Task<List<FundModel>> GetFundBalance(string accessToken);
    }

    public class FundService : IFundProfileService
    {
        private readonly FyersConfig _fyersConfig;
        public FundService(FyersConfig fyersConfig)
        {
            _fyersConfig = fyersConfig;
        }
        public async Task<List<FundModel>> GetFundBalance(string accessToken)
        {
            FyersClass stocks = FyersClass.Instance;
            stocks.ClientId = _fyersConfig.AppId;
            stocks.AccessToken = accessToken;
            Tuple<List<FundModel>, JObject> ResponseTuple = await stocks.GetFunds();
            if (ResponseTuple.Item1 == null)
            {
                return new List<FundModel>();
            }
            List<FundModel> fundDetails = ResponseTuple.Item1;
            return fundDetails;
        }
    }
}
