using Microsoft.AspNetCore.Mvc;
using TestConnectorLibary.Implementation;

namespace TestConnectorLibary.TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseEndPointController : ControllerBase
    {
        [HttpGet("/REST/candles")]
        public async Task<IActionResult> GetCandlesREST(string? pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            var tc = new TestConnector(new HttpClient());

            //test data
            pair = "BTC:USD";
            periodInSec = 60;
            from = DateTimeOffset.UtcNow.AddMonths(-12);
            to = DateTimeOffset.UtcNow;
            count = 10;

            var response = await tc.GetCandleSeriesAsync(pair, periodInSec, from, to, count);

            return Ok(response);
        }

        [HttpGet("/REST/trades")]
        public async Task<IActionResult> GetTradesREST(string? pair, int maxCount)
        {
            var tc = new TestConnector(new HttpClient());

            //test data
            pair = "BTC:USD";
            maxCount = 10;

            var response = await tc.GetNewTradesAsync(pair, maxCount);

            return Ok(response);
        }

        [HttpGet("/REST/ticker")]
        public async Task<IActionResult> GetTikerREST(string? pair)
        {
            var tc = new TestConnector(new HttpClient());

            //test data
            pair = "BTC:USD";

            var response = await tc.GetTickerAsync(pair);

            return Ok(response);
        }
    }
}
