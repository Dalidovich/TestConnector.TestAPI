using Microsoft.AspNetCore.Mvc;
using TestConnectorLibary.Interfaces;

namespace TestConnectorLibary.TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseRESTEndPointController : ControllerBase
    {
        private readonly ITestConnector _testConnector;

        public BaseRESTEndPointController(ITestConnector testConnector)
        {
            _testConnector = testConnector;
        }

        [HttpGet("/REST/candles")]
        public async Task<IActionResult> GetCandlesREST(string? pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            //test data
            pair = "BTC:USD";
            periodInSec = 60;
            from = DateTimeOffset.UtcNow.AddMonths(-12);
            to = DateTimeOffset.UtcNow;
            count = 10;

            var response = await _testConnector.GetCandleSeriesAsync(pair, periodInSec, from, to, count);

            return Ok(response);
        }

        [HttpGet("/REST/trades")]
        public async Task<IActionResult> GetTradesREST(string? pair, int maxCount)
        {
            //test data
            pair = "BTC:USD";
            maxCount = 10;

            var response = await _testConnector.GetNewTradesAsync(pair, maxCount);

            return Ok(response);
        }

        [HttpGet("/REST/ticker")]
        public async Task<IActionResult> GetTikerREST(string? pair)
        {
            //test data
            pair = "BTC:USD";

            var response = await _testConnector.GetTickerAsync(pair);

            return Ok(response);
        }
    }
}
