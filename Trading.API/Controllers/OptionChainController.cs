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
        private readonly FyersConfig _fyersConfig;
        private readonly IFyersOptionChainService _fyersOptionChainService;

        public OptionChainController(FyersConfig fyersConfig, IFyersOptionChainService fyersOptionChainService)
        {
            _fyersConfig = fyersConfig;
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
    }
}
