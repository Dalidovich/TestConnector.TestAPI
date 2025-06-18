using Microsoft.AspNetCore.Mvc;
using TestConnectorLibary.Interfaces;

namespace TestConnectorLibary.TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseWSEndPointController : ControllerBase
    {
        private readonly ITestConnector _testConnector;

        public BaseWSEndPointController(ITestConnector testConnector)
        {
            _testConnector = testConnector;
        }

        [HttpPost("/WS/candles/subscribe")]
        public async Task<IActionResult> SubscribeCandles(string? pair, int periodInSec)
        {
            //test data
            pair = "BTC:USD";
            periodInSec = 60;

            await _testConnector.SubscribeCandles(pair, periodInSec);

            return Ok();
        }

        [HttpPost("/WS/candles/unsubscribe")]
        public async Task<IActionResult> UnsubscribeCandles(string? pair)
        {
            //test data
            pair = "BTC:USD";

            await _testConnector.UnsubscribeCandles(pair);

            return Ok();
        }

        [HttpPost("/WS/trades/subscribe")]
        public async Task<IActionResult> SubscribeTrades(string? pair, int periodInSec)
        {
            //test data
            pair = "BTC:USD";
            periodInSec = 60;

            await _testConnector.SubscribeTrades(pair, periodInSec);

            return Ok();
        }

        [HttpPost("/WS/trades/unsubscribe")]
        public async Task<IActionResult> UnsubscribeTrades(string? pair)
        {
            //test data
            pair = "BTC:USD";

            await _testConnector.UnsubscribeTrades(pair);

            return Ok();
        }
    }
}
