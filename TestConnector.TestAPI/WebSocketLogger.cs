using TestConnectorLibary.Converters;
using TestConnectorLibary.Interfaces;

namespace TestConnectorLibary.TestAPI
{
    public class WebSocketLogger
    {
        ILogger<WebSocketLogger> _logger;
        ITestConnector _connector;

        public WebSocketLogger(ILogger<WebSocketLogger> logger, ITestConnector connector)
        {
            _logger = logger;
            _connector = connector;
        }

        public void SubscribeEvents()
        {
            _connector.CandleSeriesProcessing += candle =>
            {
                _logger.LogInformation($"Recive candles data({candle.Count})");
                for (var i = 0; i < candle.Count; i++)
                {
                    _logger.LogInformation($"Candle: {candle[i].ToStringFormatted()}");
                }
            };

            _connector.NewBuyTrade += trade =>
            {
                _logger.LogInformation($"Recive new BUY trade data: " +
                    $"\n\t{trade.ToStringFormatted()}");
            };

            _connector.NewSellTrade += trade =>
            {
                _logger.LogInformation($"Recive new SELL trade data: " +
                    $"\n\t{trade.ToStringFormatted()}");
            };
        }
    }
}
