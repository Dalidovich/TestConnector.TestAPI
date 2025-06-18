using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TestConnectorLibary.Converters;
using TestConnectorLibary.Entities;
using TestConnectorLibary.Entities.WebSocketMessage;
using TestConnectorLibary.Enums;

namespace TestConnectorLibary.WevSocket
{
    public class WebSocketClientParser
    {
        private ClientWebSocket _clientWebSocket;
        private CancellationTokenSource _cts;
        private SemaphoreSlim _semaphore;

        public event Action<List<Candle>> OnCandleReceived;
        public event Action<List<Trade>> OnTradeReceived;

        private Dictionary<long, WebSocketChannel> _channels = new();

        public List<string> GetActiveChannels() => _channels.Select(ch => $"Id:{ch.Key}\t type{ch.Value}").ToList();

        public async Task ConnectClientAsync()
        {
            if (_clientWebSocket == null)
            {
                _clientWebSocket = new ClientWebSocket();
                _channels.Clear();
                _cts = new CancellationTokenSource();

                await _clientWebSocket.ConnectAsync(new Uri(StandartSocketConst.baseAPIAdres), _cts.Token);
                _semaphore = new SemaphoreSlim(1, 1);
            }

            else if (_clientWebSocket.State != WebSocketState.Open)
            {
                CleanSource();
            }
        }

        public async Task DisconnectClientAsync()
        {
            if (_clientWebSocket.State == WebSocketState.Open)
            {
                await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

                CleanSource();
            }
            _channels.Clear();
        }

        public async Task<bool> ReconnectClientAsync()
        {
            await ConnectClientAsync();
            await ConnectClientAsync();
            return _clientWebSocket.State == WebSocketState.Open;
        }

        public void CleanSource()
        {
            _clientWebSocket.Dispose();
            _clientWebSocket = null;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            _semaphore.Dispose();
            _semaphore = null;
        }

        private async Task SendMessageAsync(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            await _semaphore.WaitAsync(_cts.Token);
            try
            {
                await _clientWebSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
            catch (Exception ex)
            { }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Receiver()
        {
            var buffer = new byte[16 * 1024];
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    var reciveMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (reciveMessage.StartsWith('['))
                    {

                        if (reciveMessage.EndsWith("]]]"))
                        {
                            //Snapshot Data
                            var SeparatorIndex = reciveMessage.IndexOf(',');
                            var arr = reciveMessage.Substring(SeparatorIndex);
                            arr = arr.Substring(1, arr.Length - 2);
                            var idChannel = (long)Convert.ToDouble(reciveMessage.Split(',')[0].Trim('['));
                            var channel = _channels[idChannel];
                            if (channel.Name == ChannelTypeConst.candles)
                            {
                                var candlesArray = JsonSerializer.Deserialize<List<List<decimal>>>(arr).ConvertToCandleArray(channel.Pair);
                                OnCandleReceived(candlesArray);
                            }
                            else if (channel.Name == ChannelTypeConst.trades)
                            {
                                //по факту только для будущего функционала, так как мы хотим видеть только новые сделки с разделением на buy/sell
                                //А значит snapshot нам не нужен ибо в нем могут быть и те и те, а для ручной сортировке лучше реализовать через REST
                                //var tradeArray = JsonSerializer.Deserialize<List<List<decimal>>>(arr).ConvertToTradesArray(channel.Pair);
                            }
                        }
                        else if (reciveMessage.EndsWith("]]"))
                        {
                            //Update Data
                            var SeparatorIndex = reciveMessage.IndexOf(',');
                            var arr = reciveMessage.Substring(SeparatorIndex + 1);
                            arr = arr.Substring(0, arr.Length - 1);
                            var idChannel = (long)Convert.ToDouble(reciveMessage.Split(',')[0].Trim('['));
                            var channel = _channels[idChannel];
                            if (channel.Name == ChannelTypeConst.candles)
                            {
                                var candlesArray = JsonSerializer.Deserialize<List<decimal>>(arr).ConvertToCandle(channel.Pair);
                                OnCandleReceived(new() { candlesArray });
                            }
                            else if (channel.Name == ChannelTypeConst.trades)
                            {
                                SeparatorIndex = arr.IndexOf(',');
                                arr = arr.Substring(SeparatorIndex + 1);
                                var tradesArray = JsonSerializer.Deserialize<List<decimal>>(arr).ConvertToTrade(channel.Pair);
                                OnTradeReceived(new() { tradesArray });
                            }
                        }
                    }
                    else if (reciveMessage.StartsWith('{'))
                    {
                        var specMessageResponse = JsonSerializer.Deserialize<SpecificationResponseMessage>(reciveMessage, StandartSocketConst.standartJsonSerializerOptions);
                        if (specMessageResponse.Event == MessageEventTypeConst.subscribed)
                        {
                            _channels.Add(specMessageResponse.ChanId, new WebSocketChannel(specMessageResponse));

                        }
                        else if (specMessageResponse.Event == MessageEventTypeConst.unsubscribed)
                        {
                            _channels.Remove(specMessageResponse.ChanId);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public async Task SubscribeCandlesAsync(SubscribeCandleRequestMessage requestMessage)
        {
            if (_clientWebSocket.State != WebSocketState.Open && !await ReconnectClientAsync())
            {
                return;
            }

            string json = JsonSerializer.Serialize(requestMessage, StandartSocketConst.standartJsonSerializerOptions);
            await SendMessageAsync(json);
            await Task.Factory.StartNew(async () =>
            {
                await Receiver();
            }, _cts.Token);
        }

        public async Task UnsubscribeCandlesAsync(string pair)
        {
            var channel = _channels.Values.SingleOrDefault(x => x.Pair == $"t{pair}" && x.Name == ChannelTypeConst.candles);
            if (channel == null || _clientWebSocket.State != WebSocketState.Open)
            {
                return;
            }

            var requestMessage = new UnsubscribeRequestMessage(channel.Id);
            string json = JsonSerializer.Serialize(requestMessage, StandartSocketConst.standartJsonSerializerOptions);
            await SendMessageAsync(json);
        }

        public async Task SubscribeTradesAsync(SubscribeTradeRequestMessage requestMessage)
        {
            if (_clientWebSocket.State != WebSocketState.Open && !await ReconnectClientAsync())
            {
                return;
            }
            string json = JsonSerializer.Serialize(requestMessage, StandartSocketConst.standartJsonSerializerOptions);
            await SendMessageAsync(json);
            await Task.Factory.StartNew(async () =>
            {
                await Receiver();
            }, _cts.Token);
        }

        public async Task UnsubscribeTradesAsync(string pair)
        {
            var channel = _channels.Values.SingleOrDefault(x => x.Pair == $"t{pair}" && x.Name == ChannelTypeConst.trades);
            if (channel == null || _clientWebSocket.State != WebSocketState.Open)
            {
                return;
            }

            var requestMessage = new UnsubscribeRequestMessage(channel.Id);
            string json = JsonSerializer.Serialize(requestMessage, StandartSocketConst.standartJsonSerializerOptions);
            await SendMessageAsync(json);
        }
    }
}
