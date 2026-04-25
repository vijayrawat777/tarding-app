using Microsoft.AspNetCore.Mvc;
using Trading.Infrastructure.Services;

namespace Trading.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundController : ControllerBase
    {
        private readonly IFundProfileService _fundService;
        public FundController(IFundProfileService fundService)
        {
            _fundService = fundService;
        }
        [HttpGet]
        public async Task<IActionResult> GetFundBalance(string accessToken)
        {
            // Replace with actual access token retrieval logic if needed   
            var fundBalance = await _fundService.GetFundBalance(accessToken);

            return Ok(fundBalance);
        }
    }
}
