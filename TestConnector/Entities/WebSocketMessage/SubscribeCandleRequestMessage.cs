using TestConnectorLibary.Enums;

namespace TestConnectorLibary.Entities.WebSocketMessage
{
    public class SubscribeCandleRequestMessage
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        public string Key { get; set; }

        public SubscribeCandleRequestMessage(string key)
        {
            Event = MessageEventTypeConst.subscribe;
            Channel = ChannelTypeConst.candles;
            Key = key;
        }
    }
}
