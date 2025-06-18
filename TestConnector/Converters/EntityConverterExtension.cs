using TestConnectorLibary.Entities;
using TestConnectorLibary.Entities.WebSocketMessage;
using TestConnectorLibary.Enums;

namespace TestConnectorLibary.Converters
{
    public static class EntityConverterExtension
    {
        public static List<Candle> ConvertToCandleArray(this List<List<decimal>> listOfListOfDecimals, string pair)
        {
            return listOfListOfDecimals.Select(calndle => calndle.ConvertToCandle(pair)).ToList();
        }

        public static Candle ConvertToCandle(this List<decimal> calndle, string pair)
        {
            return new Candle()
            {
                Pair = pair,
                OpenPrice = calndle[1],
                HighPrice = calndle[3],
                LowPrice = calndle[4],
                ClosePrice = calndle[2],
                TotalPrice = (calndle[1] + calndle[2] + calndle[3] + calndle[4]) / 4 * calndle[5],
                TotalVolume = calndle[5],
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)calndle[0])
            };
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

        public static List<Trade> ConvertToTradesArray(this List<List<decimal>> listOfListOfDecimals, string pair)
        {
            return listOfListOfDecimals.Select(trade => trade.ConvertToTrade(pair)).ToList();
        }

        public static Trade ConvertToTrade(this List<decimal> trade, string pair)
        {
            return new Trade()
            {
                Pair = pair,
                Price = trade[3],
                Amount = trade[2],
                Side = trade[2] > 0 ? TradeSideConst.buy : TradeSideConst.sell,
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)trade[1]),
                Id = trade[0].ToString(),
            };
        }

        public static string GetPair(this SpecificationResponseMessage specification)
        {
            if (specification.Symbol == null)
            {
                return specification.Key.Split(":")[2];
            }
            else
            {
                return specification.Symbol;
            }
        }
    }
}
