using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trading.Application.DTOs.OptionChain;
using Trading.Infrastructure.Configuration;
using Trading.Infrastructure.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Trading.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionChainController : ControllerBase
    {
        private readonly IOptionStrategyEngine _strategy;
        private readonly IFyersOptionChainService _fyersOptionChainService;

        public OptionChainController(IOptionStrategyEngine strategy, IFyersOptionChainService fyersOptionChainService)
        {
            _strategy = strategy;
            _fyersOptionChainService = fyersOptionChainService;
        }
        [HttpPost("option-chain")]
        public async Task<IActionResult> GetAllOrders([FromBody] OptionChainRequestDto request)
        {
            //"NSE:NIFTY50-INDEX"

            (OptionChainResponse chainData, decimal spotPrice) = await GetOptionChainData(request);

            if (chainData.Success)
            {
                return Ok(chainData);
            }
            else
            {
                return BadRequest(chainData.Message);
            }
        }

        [HttpPost("trade-decision")]
        public async Task<IActionResult> GetTradeSignal([FromBody] OptionChainRequestDto request)
        {
            (OptionChainResponse chainData, decimal spotPrice) = await GetOptionChainData(request);

            if (spotPrice == 0)
                return BadRequest("Spot Price not available");

            var signal = _strategy.GenerateTrades(chainData, spotPrice);

            return Ok(signal);
        }

        private async Task<(OptionChainResponse chainData, decimal spotPrice)> GetOptionChainData(OptionChainRequestDto request)
        {
            var chainData = await _fyersOptionChainService.GetOptionChainData(request);

            //if (!chainData.Success)
            //    return BadRequest(chainData.Message);

            decimal spotPrice = await _fyersOptionChainService.GetOptionChainSpotPrice(request);

            chainData.ATM = (decimal)chainData.Data.OrderBy(x => Math.Abs((decimal)x.StrikePrice - spotPrice)).First().StrikePrice;
            chainData.Support = chainData.Data.OrderByDescending(x => x.PutOpenInterest).First().PutOpenInterest;
            chainData.Resistance = chainData.Data.OrderByDescending(x => x.CallOpenInterest).First().CallOpenInterest;

            return (chainData, spotPrice);
        }
    }
}

