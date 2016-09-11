using Newtonsoft.Json.Linq;
using StockScreener2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace StockScreener2.Service
{
    public static class DataFetcher
    {
        public static string GetData(string url)
        {
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);

                return json;
            }
        }

        public static Stock GetStockQuote(string Ticker)
        {
            //Build url
            string url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%3D'" +
                Ticker + "'&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);

                Console.WriteLine(json);

                Stock stock = new Stock(json);

                return stock;
            }
         }

        public static List<HistoricalStockPrice> GetHistoricalStockPrice(string Symbol, DateTime StartDate, DateTime EndDate)
        {
            //Build url
            string url = 
                "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20%3D%20%22"
                + Symbol 
                + "%22%20and%20startDate%20%3D%20%22" 
                + StartDate.ToString("yyyy-MM-dd") 
                + "%22%20and%20endDate%20%3D%20%22" 
                + EndDate.ToString("yyyy-MM-dd") 
                + "%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);

                List<HistoricalStockPrice> result = new List<Models.HistoricalStockPrice>();
                List<string> dateArray = new List<string>();

                JObject jObject = JObject.Parse(json);
                JArray jQuotes = JArray.Parse(jObject["query"]["results"]["quote"].ToString());
                foreach (var quote in jQuotes)
                {
                    dateArray = quote["Date"].ToString().Split('-').ToList();
                    result.Add(
                        new Models.HistoricalStockPrice
                        {
                            Symbol = (string)quote["Symbol"],
                            Date = new DateTime(int.Parse(dateArray[0]), int.Parse(dateArray[1]), int.Parse(dateArray[2])),
                            Open = (decimal)quote["Open"],
                            High = (decimal)quote["High"],
                            Close = (decimal)quote["Close"],
                            Low = (decimal)quote["Low"],
                            Volume = (int)quote["Volume"],
                            AdjClose = (decimal)quote["Adj_Close"]
                        });
                }

                return result;
            }

        }
    }
}