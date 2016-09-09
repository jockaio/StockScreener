using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StockScreener2.Models
{
    public class StockPrice
    {
        public int ID { get; set; }
        public int StockID { get; set; }
        [ForeignKey("StockID")]
        public virtual Stock Stock { get; set; }

        public decimal Change { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal DaysLow { get; set; }
        public decimal DaysHigh { get; set; }
        public decimal Open { get; set; }
        public decimal? Close { get; set; }
        public DateTime Created { get; set; }
    }
}