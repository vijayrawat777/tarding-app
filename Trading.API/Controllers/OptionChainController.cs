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
        public async Task<IActionResult> GetAllOrders([FromBody] OptionChainRequestDto requestDto)
        {
            //"NSE:NIFTY50-INDEX"

            var response = await _fyersOptionChainService.GetOptionChainData(requestDto);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpPost("signal")]
        public async Task<IActionResult> GetSignal([FromBody] OptionChainRequestDto request)
        {
            var chain = await _fyersOptionChainService.GetOptionChainData(request);

            if (!chain.Success)
                return BadRequest(chain.Message);

            decimal spotPrice = await _fyersOptionChainService.GetOptionChainSpotPrice(request);

            if (spotPrice == 0)
                return BadRequest("Spot Price not available");

            var signal = _strategy.GenerateSignals(chain,spotPrice);

            return Ok(signal);
        }
    }
}

