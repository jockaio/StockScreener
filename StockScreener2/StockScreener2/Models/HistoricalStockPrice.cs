using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockScreener2.Models
{
    public class HistoricalStockPrice
    {
        public int ID { get; set; }
        public int StockID { get; set; }
        public virtual Stock Stock { get; set; }

        public string Symbol { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public int Volume { get; set; }
        public decimal AdjClose { get; set; }
    }

    public class HistoricalStockPriceComparer : IEqualityComparer<HistoricalStockPrice>
    {
        public bool Equals(HistoricalStockPrice x, HistoricalStockPrice y)
        {
            return x.Date == y.Date;
        }

        public int GetHashCode(HistoricalStockPrice obj)
        {
            return obj.Date.GetHashCode();
        }
    }
}