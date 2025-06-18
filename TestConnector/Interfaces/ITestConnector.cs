using TestConnectorLibary.Entities;

namespace TestConnectorLibary.Interfaces
{
    public interface ITestConnector
    {
        #region Rest

        Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
        Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);

        //В задании сказано об этой функциональности
        Task<Ticker> GetTickerAsync(string pair);

        #endregion


        #region Socket

        event Action<Trade> NewBuyTrade;
        event Action<Trade> NewSellTrade;

        //Заменил на Task для поддерки асинхронности
        Task SubscribeTrades(string pair, int maxCount = 100);
        Task UnsubscribeTrades(string pair);


        //Изменил Action<Candle> на Action<List<Candle>> для получения всех свечей, а не только update записей
        event Action<List<Candle>> CandleSeriesProcessing;

        //Заменил на Task для поддерки асинхронности
        //убрал параметры DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0 так как они не нужны, ибо мы подписываемя на канал с постоянным обновлением
        Task SubscribeCandles(string pair, int periodInSec);
        Task UnsubscribeCandles(string pair);

        #endregion
    }
}
