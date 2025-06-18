using Microsoft.AspNetCore.Mvc;
using TestConnectorLibary.Interfaces;

namespace TestConnectorLibary.TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BalanceEndPointController : ControllerBase
    {
        private readonly ITestConnector _testConnector;

        public BalanceEndPointController(ITestConnector testConnector)
        {
            _testConnector = testConnector;
        }

        //Здесь идет расчет баланса стараясь брать оригинальные отношения валют
        //У bitfinex нет всех отношений валют
        [HttpGet("/REST/Balance/original")]
        public async Task<IActionResult> GetTotalBalanseOriginalRateREST(decimal BTC, decimal XRP, decimal XMR, decimal DSH)
        {
            //test data
            BTC = 1m;
            XRP = 15000m;
            XMR = 50m;
            DSH = 30m;

            var balance = new Dictionary<string, decimal>
            {
                { "BTC", BTC },
                { "XRP", XRP },
                { "XMR", XMR },
                { "DSH", DSH }
            };
            var calculateCurrencyArr = new List<string>
            {
                "USD"
            };
            calculateCurrencyArr.AddRange(balance.Keys);

            var balanceSum = new Dictionary<string, decimal>();
            for (var i = 0; i < calculateCurrencyArr.Count; i++)
            {
                decimal sum = 0;
                foreach (var currency in balance)
                {
                    if (currency.Key != calculateCurrencyArr[i])
                    {
                        try
                        {
                            //попыка взять курс нужной валюты на прямую
                            var rate = (await _testConnector.GetTickerAsync($"{currency.Key}:{calculateCurrencyArr[i]}")).LastPrice;
                            sum += currency.Value * rate;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                //если не получилось на прямую, берется перевернутая пара и высчитывается на ее основе
                                var rate = (await _testConnector.GetTickerAsync($"{calculateCurrencyArr[i]}:{currency.Key}")).LastPrice;
                                sum += currency.Value / rate;
                            }
                            catch (Exception)
                            {
                                //если не получилось и в обратной паре, вычисляется их курс через USD
                                var rate = (await _testConnector.GetTickerAsync($"{currency.Key}:{calculateCurrencyArr[0]}")).LastPrice /
                                    (await _testConnector.GetTickerAsync($"{calculateCurrencyArr[i]}:{calculateCurrencyArr[0]}")).LastPrice;
                                sum += currency.Value * rate;
                            }
                        }
                    }
                    else
                    {
                        sum += currency.Value;
                    }

                }
                balanceSum.Add(calculateCurrencyArr[i], sum);
            }

            return Ok(balanceSum);
        }

        //Здесь идет расчет баланса через привязку к USD
        //У bitfinex нет всех отношений валют
        //Так что я вычесляю сумму портфеля в USD(курс к USD есть у всех)
        //И общую сумму перевожу в нужные валюты
        [HttpGet("/REST/Balance/USDBinding")]
        public async Task<IActionResult> GetTotalBalanseBindingUSDREST(decimal BTC, decimal XRP, decimal XMR, decimal DSH)
        {
            //test data
            BTC = 1m;
            XRP = 15000m;
            XMR = 50m;
            DSH = 30m;

            var BTC_USDRate = (await _testConnector.GetTickerAsync("BTC:USD")).LastPrice;
            var XRP_USDRate = (await _testConnector.GetTickerAsync("XRP:USD")).LastPrice;
            var XMR_USDRate = (await _testConnector.GetTickerAsync("XMR:USD")).LastPrice;
            var DSH_USDRate = (await _testConnector.GetTickerAsync("DSH:USD")).LastPrice;

            var balance = new Dictionary<string, decimal>
            {
                { "BTC", 1 },
                { "XRP", 15000 },
                { "XMR", 50 },
                { "DSH", 30 }
            };

            var balanseUSDSum = (BTC_USDRate * BTC) + (XRP_USDRate * XRP) + (XMR_USDRate * XMR) + (DSH_USDRate * DSH);
            var TotalBalanse = new Dictionary<string, decimal>
            {
                {"USD", balanseUSDSum },
                {"BTC", balanseUSDSum/ BTC_USDRate },
                {"XRP", balanseUSDSum/ XRP_USDRate },
                {"XMR", balanseUSDSum/ XMR_USDRate },
                {"DSH", balanseUSDSum/ DSH_USDRate },
            };

            return Ok(TotalBalanse);
        }
    }
}
