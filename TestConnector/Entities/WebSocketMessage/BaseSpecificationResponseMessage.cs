
namespace TestConnectorLibary.Entities.WebSocketMessage
{
    public abstract class BaseSpecificationResponseMessage
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        public long ChanId { get; set; }
        public string Key { get; set; }
    }
}
