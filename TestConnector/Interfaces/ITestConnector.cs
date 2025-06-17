using TestConnectorLibary.Entities;

namespace TestConnectorLibary.Interfaces
{
    interface ITestConnector
    {
        #region Rest

        Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
        Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);

        //В задании сказано об этой функциональности
        Task<Ticker> GetTickerAsync(string pair);

        #endregion


        //#region Socket

        //event Action<Trade> NewBuyTrade;
        //event Action<Trade> NewSellTrade;
        //void SubscribeTrades(string pair, int maxCount = 100);
        //void UnsubscribeTrades(string pair);

        //event Action<Candle> CandleSeriesProcessing;
        //void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        //void UnsubscribeCandles(string pair);

        //#endregion
    }
}
