
namespace TestConnectorLibary.Entities.WebSocketMessage
{
    public class SpecificationResponseMessage : BaseSpecificationResponseMessage
    {
        public string Event { get; set; }
        public string Channel { get; set; }
        public long ChanId { get; set; }
        public string Symbol { get; set; }
    }
}
