using Trading.Application.DTOs;
using Trading.Domain.Models;

namespace Trading.Application.Mappers
{
    public static class TradingMapper
    {
        public static OrderDto ToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UnderlyingSymbol = order.UnderlyingSymbol,
                OptionSymbol = order.OptionSymbol,
                IsOptionTrade = order.IsOptionTrade,
                Side = order.Side.ToString(),
                Type = order.Type.ToString(),
                Status = order.Status.ToString(),
                Quantity = order.Quantity,
                Price = order.Price,
                ExecutedQuantity = order.ExecutedQuantity,
                ExecutedPrice = order.ExecutedPrice,
                StrategyName = order.StrategyName,
                StrategyType = order.StrategyType,
                CreatedAt = order.CreatedAt,
                ExecutedAt = order.ExecutedAt,
                Notes = order.Notes
            };
        }

        public static TradeDto ToDto(Trade trade)
        {
            return new TradeDto
            {
                Id = trade.Id,
                UnderlyingSymbol = trade.UnderlyingSymbol,
                OptionSymbol = trade.OptionSymbol,
                IsOptionTrade = trade.IsOptionTrade,
                EntryDirection = trade.EntryDirection.ToString(),
                EntryPrice = trade.EntryPrice,
                Quantity = trade.Quantity,
                EntryTime = trade.EntryTime,
                ExitPrice = trade.ExitPrice,
                ExitTime = trade.ExitTime,
                Status = trade.Status.ToString(),
                StrategyName = trade.StrategyName,
                StrategyType = trade.StrategyType,
                GrossProfit = trade.GrossProfit,
                NetProfit = trade.NetProfit,
                ProfitPercent = trade.ProfitPercent,
                StopLossPrice = trade.StopLossPrice,
                TakeProfitPrice = trade.TakeProfitPrice,
                MaxDrawdown = trade.MaxDrawdown,
                CreatedAt = trade.CreatedAt,
                ClosedAt = trade.ClosedAt
            };
        }

        public static TradeLogDto ToDto(TradeLog tradeLog)
        {
            return new TradeLogDto
            {
                Id = tradeLog.Id,
                Type = tradeLog.Type.ToString(),
                Title = tradeLog.Title,
                Description = tradeLog.Description,
                TradeId = tradeLog.TradeId,
                OrderId = tradeLog.OrderId,
                StrategyName = tradeLog.StrategyName,
                Symbol = tradeLog.Symbol,
                Price = tradeLog.Price,
                Quantity = tradeLog.Quantity,
                PnL = tradeLog.PnL,
                Details = tradeLog.Details,
                Severity = tradeLog.Severity,
                CreatedAt = tradeLog.CreatedAt
            };
        }
    }
}
