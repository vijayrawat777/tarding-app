using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Trading.Domain.Models;
using Trading.Domain.Services;

namespace Trading.Infrastructure.Services
{
    public class AutomatedTradingService : IAutomatedTradingService
    {
        private readonly IOrderService _orderService;
        private readonly ITradeService _tradeService;
        private readonly ITradeLogService _tradeLogService;
        private readonly IMarketDataService _marketDataService;
        
        private bool _isRunning = false;
        private string _currentStrategy = string.Empty;
        private CancellationTokenSource _cancellationTokenSource = null!;
        private AutoTradingParameters _parameters = new();
        private Random _random = new();

        public bool IsRunning => _isRunning;
        public string CurrentStrategy => _currentStrategy;

        public event Action<string> OnStrategySignal = (_) => { };
        public event Action<string> OnAutoTradingStarted = (_) => { };
        public event Action<string> OnAutoTradingStopped = (_) => { };
        public event Action<string> OnError = (_) => { };

        public AutomatedTradingService(
            IOrderService orderService,
            ITradeService tradeService,
            ITradeLogService tradeLogService,
            IMarketDataService marketDataService)
        {
            _orderService = orderService;
            _tradeService = tradeService;
            _tradeLogService = tradeLogService;
            _marketDataService = marketDataService;
            OnError = (_) => { };
        }

        public async Task StartAutoTradingAsync(string strategyName, CancellationToken cancellationToken = default)
        {
            if (_isRunning)
            {
                OnError?.Invoke("Auto trading is already running");
                return;
            }

            _isRunning = true;
            _currentStrategy = strategyName;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            OnAutoTradingStarted?.Invoke(strategyName);

            try
            {
                await RunTradingLoopAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Error in trading loop: " + ex.Message);
            }
        }

        public async Task StopAutoTradingAsync()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            OnAutoTradingStopped?.Invoke(_currentStrategy);
            _currentStrategy = string.Empty;

            await Task.CompletedTask;
        }

        public async Task ExecuteStrategyAsync(string strategyName)
        {
            var signal = await GenerateStrategySignalAsync(strategyName);
            OnStrategySignal?.Invoke(signal.Action + " " + signal.Symbol);

            try
            {
                var order = new Order
                {
                    UnderlyingSymbol = signal.Symbol,
                    Side = signal.Action == "BUY" ? OrderSide.Buy : OrderSide.Sell,
                    Type = OrderType.Market,
                    Quantity = signal.Quantity,
                    Price = signal.Price,
                    StrategyName = strategyName,
                    StrategyType = strategyName
                };

                var placedOrder = await _orderService.PlaceOrderAsync(order);

                var trade = new Trade
                {
                    UnderlyingSymbol = signal.Symbol,
                    EntryDirection = order.Side,
                    EntryPrice = signal.Price,
                    Quantity = signal.Quantity,
                    EntryTime = DateTime.UtcNow,
                    EntryOrderId = placedOrder.Id,
                    Status = TradeStatus.Open,
                    StopLossPrice = signal.Price * 0.98m,
                    TakeProfitPrice = signal.Price * 1.05m,
                    StrategyName = strategyName,
                    StrategyType = strategyName
                };

                await _tradeService.OpenTradeAsync(trade);

                var log = new TradeLog
                {
                    Type = TradeLogType.StrategySignal,
                    Title = "Strategy Signal: " + strategyName,
                    Description = signal.Reason,
                    Symbol = signal.Symbol,
                    Price = signal.Price,
                    Quantity = signal.Quantity,
                    StrategyName = strategyName,
                    Severity = "Info"
                };

                await _tradeLogService.LogAsync(log);
            }
            catch (Exception ex)
            {
                OnError?.Invoke("Error executing strategy: " + ex.Message);

                var errorLog = new TradeLog
                {
                    Type = TradeLogType.StrategyError,
                    Title = "Strategy Error: " + strategyName,
                    Description = ex.Message,
                    Symbol = signal.Symbol,
                    StrategyName = strategyName,
                    Severity = "Error"
                };

                await _tradeLogService.LogAsync(errorLog);
            }
        }

        public async Task SetAutoTradingParametersAsync(AutoTradingParameters parameters)
        {
            _parameters = parameters;
            await Task.CompletedTask;
        }

        public async Task<AutoTradingParameters> GetAutoTradingParametersAsync()
        {
            await Task.CompletedTask;
            return _parameters;
        }

        private async Task RunTradingLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    await ExecuteStrategyAsync(_currentStrategy);

                    var intervalSeconds = _parameters?.ExecutionIntervalSeconds ?? 60;
                    await Task.Delay(intervalSeconds * 1000, cancellationToken);

                    await CheckAndCloseTradesAsync();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    OnError?.Invoke("Trading loop error: " + ex.Message);
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private async Task CheckAndCloseTradesAsync()
        {
            var openTrades = await _tradeService.GetOpenTradesAsync();

            foreach (var trade in openTrades)
            {
                var stock = await _marketDataService.GetStockAsync(trade.UnderlyingSymbol);
                if (stock == null) continue;
                
                var currentPrice = stock.CurrentPrice;

                if (currentPrice >= trade.TakeProfitPrice)
                {
                    await _tradeService.CloseTradeAsync(trade.Id, currentPrice);

                    var tpLog = new TradeLog
                    {
                        Type = TradeLogType.TakeProfitTriggered,
                        Title = "Take Profit Hit",
                        Description = "Price reached take profit level at " + currentPrice,
                        TradeId = trade.Id,
                        Symbol = trade.UnderlyingSymbol,
                        Price = currentPrice,
                        Severity = "Success"
                    };

                    await _tradeLogService.LogAsync(tpLog);
                }
                else if (currentPrice <= trade.StopLossPrice)
                {
                    await _tradeService.CloseTradeAsync(trade.Id, currentPrice);

                    var slLog = new TradeLog
                    {
                        Type = TradeLogType.StopLossTriggered,
                        Title = "Stop Loss Hit",
                        Description = "Price hit stop loss level at " + currentPrice,
                        TradeId = trade.Id,
                        Symbol = trade.UnderlyingSymbol,
                        Price = currentPrice,
                        Severity = "Error"
                    };

                    await _tradeLogService.LogAsync(slLog);
                }
            }
        }

        private async Task<StrategySignal> GenerateStrategySignalAsync(string strategyName)
        {
            var symbols = new[] { "RELIANCE", "INFY", "TCS", "HDFCBANK", "HDFC", "LT", "BAJAJFINSV", "ITC", "MARUTI", "SBIN" };
            var symbol = symbols[_random.Next(symbols.Length)];
            var stock = await _marketDataService.GetStockAsync(symbol);
            var price = stock?.CurrentPrice ?? 100m;
            var action = _random.NextDouble() > 0.5 ? "BUY" : "SELL";
            var quantity = _random.Next(1, 50);

            return new StrategySignal
            {
                Symbol = symbol,
                Action = action,
                Price = price,
                Quantity = quantity,
                Reason = strategyName + " signal generated"
            };
        }

        private class StrategySignal
        {
            public string Symbol { get; set; } = "";
            public string Action { get; set; } = "";
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string Reason { get; set; } = "";
        }
    }
}


