using System.Text.Json;
using TestConnectorLibary.Builders;
using TestConnectorLibary.Converters;
using TestConnectorLibary.Entities;
using TestConnectorLibary.Enums;
using TestConnectorLibary.Interfaces;

namespace TestConnectorLibary.Implementation
{
    public class TestConnector : ITestConnector
    {
        private readonly HttpClient _httpClient;

        public TestConnector(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            var candles = new List<Candle>();

            var periodTime = BaseConverter.PeriodInSecToString(periodInSec);
            var FormatedPair = BaseConverter.FormatingPair(pair);

            var url = new BaseRequestBuilder($"{StandartConst.baseAPIAdres}{StandartConst.candlesUrl}")
                .BuildPeriodTimeAndPair(periodTime, FormatedPair)
                .BuildHist()
                .BuildStart(from)
                .BuildEnd(to)
                .BuildLimit(count)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var candlesData = JsonSerializer.Deserialize<List<List<decimal>>>(responseBody);

            candles.AddRange(candlesData.ConvertToCandle(FormatedPair));

            return candles;
        }

        public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            var trades = new List<Trade>();
            var FormatedPair = BaseConverter.FormatingPair(pair);

            var url = new BaseRequestBuilder($"{StandartConst.baseAPIAdres}{StandartConst.TradeUrl}")
                .BuildPair(FormatedPair)
                .BuildHist()
                .BuildLimit(maxCount)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var tradesData = JsonSerializer.Deserialize<List<List<decimal>>>(responseBody);

            trades.AddRange(tradesData.ConvertToTrade(FormatedPair));

            return trades;
        }

        public async Task<Ticker> GetTickerAsync(string pair)
        {
            var FormatedPair = BaseConverter.FormatingPair(pair);
            var url = new BaseRequestBuilder($"{StandartConst.baseAPIAdres}{StandartConst.TickerUrl}")
                .BuildPair(FormatedPair)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var tradesData = JsonSerializer.Deserialize<List<decimal>>(responseBody);

            return tradesData.ConvertToTicker(FormatedPair);
        }
    }
}
