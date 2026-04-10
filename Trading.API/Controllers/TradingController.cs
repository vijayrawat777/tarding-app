using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trading.Application.DTOs;
using Trading.Application.Mappers;
using Trading.Domain.Models;
using Trading.Domain.Services;

namespace Trading.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradingController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITradeService _tradeService;
        private readonly ITradeLogService _tradeLogService;
        private readonly IAutomatedTradingService _automatedTradingService;

        public TradingController(
            IOrderService orderService,
            ITradeService tradeService,
            ITradeLogService tradeLogService,
            IAutomatedTradingService automatedTradingService)
        {
            _orderService = orderService;
            _tradeService = tradeService;
            _tradeLogService = tradeLogService;
            _automatedTradingService = automatedTradingService;
        }

        [HttpPost("orders/place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            var order = new Order
            {
                UnderlyingSymbol = request.UnderlyingSymbol,
                OptionSymbol = request.OptionSymbol,
                IsOptionTrade = request.IsOptionTrade,
                Side = request.Side == "Buy" ? OrderSide.Buy : OrderSide.Sell,
                Type = System.Enum.Parse<OrderType>(request.Type),
                Quantity = request.Quantity,
                Price = request.Price,
                StrategyName = request.StrategyName,
                StrategyType = request.StrategyType,
                Notes = request.Notes
            };

            var placedOrder = await _orderService.PlaceOrderAsync(order);
            
            var log = new TradeLog
            {
                Type = TradeLogType.OrderPlaced,
                Title = request.Side + " " + request.UnderlyingSymbol,
                Description = "Order placed",
                OrderId = placedOrder.Id,
                Symbol = request.UnderlyingSymbol,
                Price = request.Price,
                Quantity = request.Quantity,
                StrategyName = request.StrategyName
            };
            await _tradeLogService.LogAsync(log);

            return Ok(TradingMapper.ToDto(placedOrder));
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders.Select(o => TradingMapper.ToDto(o)).ToList());
        }

        [HttpGet("trades")]
        public async Task<IActionResult> GetAllTrades()
        {
            var trades = await _tradeService.GetAllTradesAsync();
            return Ok(trades.Select(t => TradingMapper.ToDto(t)).ToList());
        }

        [HttpPost("trades/{tradeId}/close")]
        public async Task<IActionResult> CloseTrade(string tradeId, [FromBody] CloseTradeRequest request)
        {
            var trade = await _tradeService.CloseTradeAsync(tradeId, request.ExitPrice);
            if (trade == null) return BadRequest("Cannot close trade");
            return Ok(TradingMapper.ToDto(trade));
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int limit = 100)
        {
            var logs = await _tradeLogService.GetLogsAsync(limit);
            return Ok(logs.Select(l => TradingMapper.ToDto(l)).ToList());
        }

        [HttpPost("auto-trading/start")]
        public async Task<IActionResult> StartAutoTrading([FromBody] StartAutoTradingRequest request)
        {
            if (_automatedTradingService.IsRunning)
                return BadRequest("Auto trading already running");

            var cts = new CancellationTokenSource();
            await _automatedTradingService.StartAutoTradingAsync(request.StrategyName, cts.Token);
            return Ok(new { message = "Started" });
        }

        [HttpPost("auto-trading/stop")]
        public async Task<IActionResult> StopAutoTrading()
        {
            await _automatedTradingService.StopAutoTradingAsync();
            return Ok(new { message = "Stopped" });
        }

        [HttpGet("auto-trading/status")]
        public IActionResult GetAutoTradingStatus()
        {
            return Ok(new
            {
                IsRunning = _automatedTradingService.IsRunning,
                CurrentStrategy = _automatedTradingService.CurrentStrategy
            });
        }

        [HttpGet("trades/summary")]
        public async Task<IActionResult> GetTradeSummary()
        {
            var totalPnL = await _tradeService.GetTotalPnLAsync();
            var totalTrades = await _tradeService.GetTotalTradesCountAsync();
            var openTrades = await _tradeService.GetOpenTradesAsync();

            return Ok(new
            {
                TotalTrades = totalTrades,
                OpenTrades = openTrades.Count(),
                TotalPnL = totalPnL,
                TotalPnLPercent = (totalPnL / 100000m) * 100
            });
        }
    }

    public class PlaceOrderRequest
    {
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; }
        public bool IsOptionTrade { get; set; }
        public string Side { get; set; }
        public string Type { get; set; } = "Market";
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string StrategyName { get; set; }
        public string StrategyType { get; set; }
        public string Notes { get; set; }
    }

    public class CloseTradeRequest { public decimal ExitPrice { get; set; } }

    public class StartAutoTradingRequest { public string StrategyName { get; set; } }
}
