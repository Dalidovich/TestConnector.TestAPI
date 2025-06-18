using TestConnectorLibary.Entities;

namespace TestConnectorLibary.Converters
{
    public static class EntityViewFormatedExtension
    {
        public static string ToStringFormatted(this Candle candle)
        {
            return $"Candle: " +
                $"\n\tPair={candle.Pair}," +
                $"\n\tOpenPrice={candle.OpenPrice}, " +
                $"\n\tHighPrice={candle.HighPrice}, " +
                $"\n\tLowPrice={candle.LowPrice}, " +
                $"\n\tClosePrice={candle.ClosePrice}, " +
                $"\n\tTotalPrice={candle.TotalPrice}, " +
                $"\n\tTotalVolume={candle.TotalVolume}, " +
                $"\n\tOpenTime={candle.OpenTime}\n";
        }

        public static string ToStringFormatted(this Trade trade)
        {
            return $"Trade: " +
                $"\n\tPair={trade.Pair}," +
                $"\n\tOpenPrice={trade.Price}, " +
                $"\n\tHighPrice={trade.Amount}, " +
                $"\n\tLowPrice={trade.Side}, " +
                $"\n\tClosePrice={trade.Time}, " +
                $"\n\tTotalPrice={trade.Id}";
        }
    }
}
