using TestConnectorLibary.Enums;

namespace TestConnectorLibary.Entities.WebSocketMessage
{
    public class SubscribeTradeRequestMessage
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        public string Symbol { get; set; }

        public SubscribeTradeRequestMessage(string symbol)
        {
            Event = MessageEventTypeConst.subscribe;
            Channel = ChannelTypeConst.trades;
            Symbol = symbol;
        }
    }
}
