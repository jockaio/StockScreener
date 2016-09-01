using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockScreener2.Models
{
    public class Stock
    {
        public Stock() { }

        public Stock(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jStock = jObject["query"]["results"]["quote"];
            Name = (string)jStock["Name"];
            Ticker = (string)jStock["symbol"];
            StockPrices = new List<StockPrice>();
            StockPrices.Add(new StockPrice()
            {
                Ask = (decimal)jStock["Ask"],
                Bid = (decimal)jStock["Bid"],
                DaysHigh = (decimal)jStock["DaysHigh"],
                DaysLow = (decimal)jStock["DaysLow"],
                Open = (decimal)jStock["Open"],
                Created = DateTime.Now

            });
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public virtual ICollection<StockPrice> StockPrices { get; set; }
    }
}