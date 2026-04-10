using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Trading.Domain.Services
{
    public interface IAutomatedTradingService
    {
        // Control
        Task StartAutoTradingAsync(string strategyName, CancellationToken cancellationToken);
        Task StopAutoTradingAsync();
        bool IsRunning { get; }
        string CurrentStrategy { get; }
        
        // Strategy Execution
        Task ExecuteStrategyAsync(string strategyName);
        
        // Settings
        Task SetAutoTradingParametersAsync(AutoTradingParameters parameters);
        Task<AutoTradingParameters> GetAutoTradingParametersAsync();
        
        // Events
        event Action<string> OnStrategySignal;
        event Action<string> OnAutoTradingStarted;
        event Action<string> OnAutoTradingStopped;
        event Action<string> OnError;
    }

    public class AutoTradingParameters
    {
        public decimal MaxPositionSize { get; set; } = 100;
        public decimal StopLossPercent { get; set; } = 2;
        public decimal TakeProfitPercent { get; set; } = 5;
        public int MaxConcurrentTrades { get; set; } = 5;
        public int ExecutionIntervalSeconds { get; set; } = 60;
        public bool AutoCloseProfitableTrades { get; set; } = true;
        public bool AutoCloseLosingTrades { get; set; } = true;
    }
}
