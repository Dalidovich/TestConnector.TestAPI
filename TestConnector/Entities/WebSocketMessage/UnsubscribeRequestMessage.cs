using TestConnectorLibary.Enums;

namespace TestConnectorLibary.Entities.WebSocketMessage
{
    public class UnsubscribeRequestMessage
    {
        public string Event { get; set; }
        public long ChanId { get; set; }

        public UnsubscribeRequestMessage(long id)
        {
            Event = MessageEventTypeConst.unsubscribe;
            ChanId = id;
        }
    }
}
