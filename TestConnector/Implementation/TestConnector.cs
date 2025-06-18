using System.Text.Json;
using TestConnectorLibary.Builders;
using TestConnectorLibary.Converters;
using TestConnectorLibary.Entities;
using TestConnectorLibary.Entities.WebSocketMessage;
using TestConnectorLibary.Enums;
using TestConnectorLibary.Interfaces;
using TestConnectorLibary.WevSocket;

namespace TestConnectorLibary.Implementation
{
    public class TestConnector : ITestConnector
    {
        private readonly HttpClient _httpClient;
        private readonly WebSocketClientParser _wsClient;

        public TestConnector(HttpClient httpClient, WebSocketClientParser webSocketClient)
        {
            _httpClient = httpClient;
            _wsClient = webSocketClient;
            _wsClient.ConnectClientAsync().Wait();

            _wsClient.OnCandleReceived += (candles) =>
            {
                CandleSeriesProcessing?.Invoke(candles);
            };

            _wsClient.OnTradeReceived += (trades) =>
            {
                if (trades[0].Side == TradeSideConst.buy)
                {
                    NewBuyTrade?.Invoke(trades[0]);
                }
                else
                {
                    NewSellTrade?.Invoke(trades[0]);
                }
            };
        }

        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            var candles = new List<Candle>();

            var periodTime = BaseConverter.PeriodInSecToString(periodInSec);
            var FormatedPair = BaseConverter.FormatingPair(pair);

            var url = new BaseRequestBuilder($"{StandartRESTConst.baseAPIAdres}{StandartRESTConst.candlesUrl}")
                .BuildPeriodTimeAndPair(periodTime, FormatedPair)
                .BuildHist()
                .BuildStart(from)
                .BuildEnd(to)
                .BuildLimit(count)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var candlesData = JsonSerializer.Deserialize<List<List<decimal>>>(responseBody);

            candles.AddRange(candlesData.ConvertToCandleArray(FormatedPair));

            return candles;
        }

        public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            var trades = new List<Trade>();
            var FormatedPair = BaseConverter.FormatingPair(pair);

            var url = new BaseRequestBuilder($"{StandartRESTConst.baseAPIAdres}{StandartRESTConst.TradeUrl}")
                .BuildPair(FormatedPair)
                .BuildHist()
                .BuildLimit(maxCount)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var tradesData = JsonSerializer.Deserialize<List<List<decimal>>>(responseBody);

            trades.AddRange(tradesData.ConvertToTradesArray(FormatedPair));

            return trades;
        }

        public async Task<Ticker> GetTickerAsync(string pair)
        {
            var FormatedPair = BaseConverter.FormatingPair(pair);
            var url = new BaseRequestBuilder($"{StandartRESTConst.baseAPIAdres}{StandartRESTConst.TickerUrl}")
                .BuildPair(FormatedPair)
                .Build();

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();

            var tradesData = JsonSerializer.Deserialize<List<decimal>>(responseBody);

            return tradesData.ConvertToTicker(FormatedPair);
        }

        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public event Action<List<Candle>> CandleSeriesProcessing;

        public async Task SubscribeCandles(string pair, int periodInSec)
        {
            var periodTime = BaseConverter.PeriodInSecToString(periodInSec);
            var FormatedPair = BaseConverter.FormatingPair(pair);
            var message = new SubscribeCandleRequestMessage($"trade:{periodTime}:t{FormatedPair}");
            await _wsClient.SubscribeCandlesAsync(message);
        }

        public async Task SubscribeTrades(string pair)
        {
            var FormatedPair = BaseConverter.FormatingPair(pair);
            var message = new SubscribeTradeRequestMessage(FormatedPair);
            await _wsClient.SubscribeTradesAsync(message);
        }

        public async Task UnsubscribeCandles(string pair)
        {
            var FormatedPair = BaseConverter.FormatingPair(pair);
            await _wsClient.UnsubscribeCandlesAsync(FormatedPair);
        }

        public async Task UnsubscribeTrades(string pair)
        {
            var FormatedPair = BaseConverter.FormatingPair(pair);
            await _wsClient.UnsubscribeTradesAsync(FormatedPair);
        }
    }
}
