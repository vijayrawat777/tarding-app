using FyersCSharpSDK;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trading.Infrastructure.Services.Swing;
using Trading.StrategyEngine.Engine.Swing;

namespace Trading.API.Controllers
{
    [ApiController]
    [Route("api/swing")]
    public class SwingController : ControllerBase
    {
        private readonly IFyersService _fyers;
        private readonly SwingStrategy _strategy;

        public SwingController(IFyersService fyers)
        {
            _fyers = fyers;
            _strategy = new SwingStrategy();
        }

        [HttpPost("stock-history")]
        public async Task<IActionResult> StockHistory([FromBody]StockHistoryModel model, string accessToken)
        {
            var candles = await _fyers.GetHistoricalData(accessToken, model);
            var signal = _strategy.Generate(model.Symbol, candles);

            return Ok(signal);
        }
    }
}
