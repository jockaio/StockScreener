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

        public static Stock GetStock(string Ticker)
        {
            //Build url
            string url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%3D'" +
                Ticker + "'&format=json&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);

                Console.WriteLine(json);

                Stock stock = new Stock(json);

                return stock;
            }
         }
    }
}