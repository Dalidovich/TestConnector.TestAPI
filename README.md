# Test Connector Libary
## Описание проекта

Connector Libary для API биржи (в Данном случае используется Bitfinex)

+ Библиотека
+ ASP WEB API + Swagger для просмотра и тестирования функциональности

## Библиотека

Библиотека реализует следующий интерфес:
```CSharp
public interface ITestConnector
{
    #region Rest
    Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
    Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);
    Task<Ticker> GetTickerAsync(string pair);
    #endregion


    #region Socket
    event Action<Trade> NewBuyTrade;
    event Action<Trade> NewSellTrade;
    Task SubscribeTrades(string pair, int maxCount = 100);
    Task UnsubscribeTrades(string pair);

    event Action<List<Candle>> CandleSeriesProcessing;
    Task SubscribeCandles(string pair, int periodInSec);
    Task UnsubscribeCandles(string pair);
    #endregion
}
```

## ASP WEB API + Swagger для просмотра и тестирования функциональности

В ASP API реализованы контроллеры для вызова функциональности библиотеки
### Base REST endpoint Controller
```bash
curl -X 'GET' \
  'https://localhost:7232/REST/candles?pair=BTC%3AUSD&periodInSec=60&from=2024-06-18T15%3A35%3A46%2B03%3A00&to=2025-06-18T15%3A35%3A46%2B03%3A00&count=10'\
  -H 'accept: */*'
```
```bash
curl -X 'GET' \
  'https://localhost:7232/REST/trades?pair=BTC%3AUSD&maxCount=10' \
  -H 'accept: */*'
```
```bash
curl -X 'GET' \
  'https://localhost:7232/REST/ticker?pair=BTC%3AUSD' \
  -H 'accept: */*'
```



### Base Websocket endpoint Controller
```bash
curl -X 'POST' \
  'https://localhost:7232/WS/candles/subscribe?pair=BTC%3AUSD&periodInSec=60' \
  -H 'accept: */*' \
  -d ''
```
```bash
curl -X 'POST' \
  'https://localhost:7232/WS/candles/unsubscribe?pair=BTC%3AUSD' \
  -H 'accept: */*' \
  -d ''
```
```bash
curl -X 'POST' \
  'https://localhost:7232/WS/trades/subscribe?pair=BTC%3AUSD' \
  -H 'accept: */*' \
  -d ''
```
```bash
curl -X 'POST' \
  'https://localhost:7232/WS/candles/subscribe?pair=BTC%3AUSD&periodInSec=60' \
  -H 'accept: */*' \
  -d ''
```
```bash
curl -X 'POST' \
  'https://localhost:7232/WS/trades/unsubscribe?pair=BTC%3AUSD' \
  -H 'accept: */*' \
  -d ''
```



### Balance endpoint Controller
Дополнительный контроллер демонстрирующий работу библиотеки
Реализуе расчет баланса 4 криптовалют, в общую сумму в каждой из валют + USD.
```bash
curl -X 'GET' \
  'https://localhost:7232/REST/Balance/original?BTC=1&XRP=1&XMR=1&DSH=1' \
  -H 'accept: */*'
```
```bash
curl -X 'GET' \
  'https://localhost:7232/REST/Balance/USDBinding?BTC=1&XRP=1&XMR=1&DSH=1' \
  -H 'accept: */*'
```
Пояснение  
Метод **'REST/Balance/original'** старается использовать оригинальные курсы валют или расчет на основании имеющихся  
Метод **'REST/Balance/USDBinding'** находит общую сумму в USD а после конвертирует ее для каждой валюты
### Дополнительно
В меотдах контроллера на текущей версии захардкодено тестовые параметры(что бы при проверке кажды раз не вводить)
```CSharp
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
```