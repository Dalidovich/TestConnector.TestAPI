using System.Text;

namespace TestConnectorLibary.Builders
{
    public class BaseRequestBuilder
    {
        private StringBuilder _Instance;
        private BaseRequestBuilder _InstanceBuilder;

        public BaseRequestBuilder(string instance)
        {
            _Instance = new StringBuilder(instance);
            _InstanceBuilder = this;
        }

        public BaseRequestBuilder BuildPeriodTimeAndPair(string periodTime, string pair)
        {
            _Instance.Append($":{periodTime}:t{pair}/");

            return _InstanceBuilder;
        }

        public BaseRequestBuilder BuildPair(string pair)
        {
            _Instance.Append($"t{pair}/");

            return _InstanceBuilder;
        }

        public BaseRequestBuilder BuildHist()
        {
            _Instance.Append($"hist?");

            return _InstanceBuilder;
        }

        public BaseRequestBuilder BuildLimit(long? count)
        {
            _Instance.Append($"limit={count}&");

            return _InstanceBuilder;
        }

        public BaseRequestBuilder BuildStart(DateTimeOffset? from)
        {
            if (from != null)
            {
                _Instance.Append($"start={from?.ToUnixTimeMilliseconds()}&");
            }

            return _InstanceBuilder;
        }

        public BaseRequestBuilder BuildEnd(DateTimeOffset? to)
        {
            if (to != null)
            {
                _Instance.Append($"end={to?.ToUnixTimeMilliseconds()}&");
            }

            return _InstanceBuilder;
        }

        public string Build() => _Instance.ToString();
    }
}
