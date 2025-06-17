namespace TestConnectorLibary.Converters
{
    public static class BaseConverter
    {
        public static string FormatingPair(string pair) => string.Join("", pair.Split(':'));

        private static Dictionary<int, string> TimeAvailableValuesDictionary = new Dictionary<int, string>
            {
                { 60, "1m" },
                { 300, "5m" },
                { 900, "15m" },
                { 1800, "30m" },
                { 3600, "1h" },
                { 3600*3, "3h" },
                { 3600*6, "6h" },
                { 3600*12, "12h" },
                { 3600*24, "1D" },
                { 3600*24*7, "1W" },
                { 3600*24*14, "14D" },
                { 3600*24*30, "1M" }
            };

        public static string PeriodInSecToString(int period)
        {
            foreach (var item in TimeAvailableValuesDictionary)
            {
                if (period < item.Key)
                {
                    return item.Value;
                }
            }
            return TimeAvailableValuesDictionary[3600 * 24 * 30];
        }
    }
}
