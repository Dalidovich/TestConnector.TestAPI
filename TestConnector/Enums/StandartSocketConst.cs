using System.Text.Json;

namespace TestConnectorLibary.Enums
{
    public static class StandartSocketConst
    {
        public const string baseAPIAdres = "wss://api-pub.bitfinex.com/ws/2";


        public readonly static JsonSerializerOptions standartJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
