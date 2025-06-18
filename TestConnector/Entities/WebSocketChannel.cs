using TestConnectorLibary.Converters;
using TestConnectorLibary.Entities.WebSocketMessage;

namespace TestConnectorLibary.Entities
{
    public class WebSocketChannel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Pair { get; set; }

        public WebSocketChannel(long id, string name, string pair)
        {
            Id = id;
            Name = name;
            Pair = pair;
        }

        public WebSocketChannel(SpecificationResponseMessage specMessageResponse)
        {
            Id = specMessageResponse.ChanId;
            Name = specMessageResponse.Channel;
            Pair = specMessageResponse.GetPair();
        }
    }
}
