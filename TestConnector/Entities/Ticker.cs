namespace TestConnectorLibary.Entities
{
    public class Ticker
    {
        /// <summary>
        /// Валютная пара
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Цена последней наивысшей ставки
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// Сумма 25 самых высоких размеров ставок
        /// </summary>
        public decimal BidSize { get; set; }

        /// <summary>
        /// Цена последнего самого низкого предложения
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// Сумма 25 самых низких размеров спроса
        /// </summary>
        public decimal AskSize { get; set; }

        /// <summary>
        /// На сколько изменилась последняя цена со вчерашнего дня
        /// </summary>
        public decimal DailyChange { get; set; }

        /// <summary>
        /// Относительное изменение цены со вчерашнего дня (*100 для процентного изменения)
        /// </summary>
        public decimal DailyChangeRelative { get; set; }

        /// <summary>
        /// Цена последней сделки
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Ежедневный объем
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Дневной максимум
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Дневной минимум
        /// </summary>
        public decimal Low { get; set; }

    }
}
