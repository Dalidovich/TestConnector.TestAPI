using TestConnectorLibary.Entities;

namespace TestConnectorLibary.Converters
{
    public static class EntityConverter
    {
        public static List<Candle> ConvertToCandle(this List<List<decimal>> listOfDecimals, string pair)
        {
            return listOfDecimals.Select(calndle => new Candle()
            {
                Pair = pair,
                OpenPrice = calndle[1],
                HighPrice = calndle[3],
                LowPrice = calndle[4],
                ClosePrice = calndle[2],
                TotalPrice = (calndle[1] + calndle[2] + calndle[3] + calndle[4]) / 4 * calndle[5],
                TotalVolume = calndle[5],
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)calndle[0])
            }).ToList();
        }

        public static Ticker ConvertToTicker(this List<decimal> ticker, string pair)
        {
            return new Ticker()
            {
                Pair = pair,
                Bid = ticker[0],
                BidSize = ticker[1],
                Ask = ticker[2],
                AskSize = ticker[3],
                DailyChange = ticker[4],
                DailyChangeRelative = ticker[5],
                LastPrice = ticker[6],
                Volume = ticker[7],
                High = ticker[8],
                Low = ticker[9],
            };
        }

        public static List<Trade> ConvertToTrade(this List<List<decimal>> listOfDecimals, string pair)
        {
            return listOfDecimals.Select(trade => new Trade()
            {
                Pair = pair,
                Price = trade[3],
                Amount = trade[2],
                Side = trade[2] > 0 ? "buy" : "sell",
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)trade[1]),
                Id = trade[0].ToString(),
            }).ToList();
        }
    }
}
