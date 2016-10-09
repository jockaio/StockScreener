using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using StockScreener2.Models;
using StockScreener2.Service;

namespace StockScreener2.Controllers
{
    [Authorize]
    public class StocksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Stocks
        public List<Stock> GetStocks(bool forceRefresh)
        {
            List<Stock> result = new List<Stock>();

            List<Stock> stocks = db.Stocks.ToList();

            ApplicationDbContext stockPriceContext = new ApplicationDbContext();

            StockPrice stockPrice = null;

            foreach (var stock in stocks)
            {
                stockPrice = stockPriceContext.StockPrices.Where(s => s.StockID == stock.ID).OrderByDescending(s => s.Created).First();

                if ((stockPrice == null || stockPrice.Created < DateTime.Now.AddHours(-1)) || forceRefresh)
                {
                    try
                    {
                        stockPrice = DataFetcher.GetStockQuote(stock.Symbol).StockPrices.FirstOrDefault();
                    }
                    catch (Exception)
                    {

                    }
                    stockPrice.StockID = stock.ID;
                    stockPriceContext.StockPrices.Add(stockPrice);
                }

                stockPrice.CalculateValues();

                if (stock.StockPrices.Count > 0)
                {
                    stock.StockPrices.Clear();
                    stock.StockPrices.Add(stockPrice);
                }
                result.Add(stock);
            }

            stockPriceContext.SaveChanges();
            
            return result.OrderBy(res => res.Name).ToList();
        }

        // GET: api/Stocks/5
        [ResponseType(typeof(Stock))]
        public IHttpActionResult GetStock(int id)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // PUT: api/Stocks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStock(int id, Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stock.ID)
            {
                return BadRequest();
            }

            db.Entry(stock).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks
        [ResponseType(typeof(Stock))]
        public IHttpActionResult PostStock(string stockSymbol)
        {
            Stock stock = null;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!db.Stocks.Any(s => s.Symbol == stockSymbol))
            {
                try
                {
                    stock = DataFetcher.GetStockQuote(stockSymbol);
                }
                catch (Exception)
                {

                    return Content(HttpStatusCode.BadRequest, "Stock symbol not found.");
                }
                
                db.Stocks.Add(stock);
                db.SaveChanges();
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, "The stock you are trying to add is already in the list.");
            }

            return CreatedAtRoute("DefaultApi", new { id = stock.ID }, stock);
        }

        // DELETE: api/Stocks/5
        [ResponseType(typeof(Stock))]
        public IHttpActionResult DeleteStock(int id)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return NotFound();
            }

            db.Stocks.Remove(stock);
            db.SaveChanges();

            return Ok(stock);
        }

        //GET: api/Stocks/GetHistoricalQuotes
        [Route("api/Stocks/GetHistoricalQuotes/{id}/{days?}")]
        [ResponseType(typeof(List<HistoricalStockPrice>))]
        public IHttpActionResult GetHistoricalQuotes(int id, int days = 7)
        {
            string stockSymbol = db.Stocks.Where(s => s.ID == id).Select(s => s.Symbol).First();

            if (stockSymbol == null)
            {
                return Content(HttpStatusCode.BadRequest, "Stock symbol not recognized.");
            }
            
            List<HistoricalStockPrice> result = new List<HistoricalStockPrice>();

            //Create startDate to be able to submit LINQ expression.
            DateTime startDate = DateTime.Now.AddDays(-days);
            List<HistoricalStockPrice> dbResult = db.HistoricalStockPrices.Where(hsp => hsp.StockID == id && hsp.Date >= startDate).OrderBy(s => s.Date).ToList();

            //if dbResult does not find the total number of days.
            if (dbResult.Count < ((Helper.GetWorkingDays(startDate, DateTime.Now))-1))
            {
                //Get historical quotes from API and save the missing dates in the DB.
                try
                {
                    List<HistoricalStockPrice> apiResult = DataFetcher.GetHistoricalStockPrice(stockSymbol, DateTime.Now.AddDays(-days), DateTime.Now, id).OrderBy(s => s.Date).ToList();
                    //var comparedResult = apiResult.Except(dbResult, new HistoricalStockPriceComparer()).ToList();
                    PostHistoricalStockPrices(apiResult.Except(dbResult, new HistoricalStockPriceComparer()).ToList());
                    result = apiResult;
                }
                catch (Exception)
                {

                    return Content(HttpStatusCode.BadRequest, "Historical quotes not available at the moment.");
                }
            }
            else
            {
                //Othervise the dbResult is sufficient.
                result = dbResult;
            }

            //Add todays stock price to include it in the chart.
            StockPrice stockPrice = db.StockPrices.Where(st => st.StockID == id).OrderByDescending(st => st.Created).First();
            result.Add(
                new HistoricalStockPrice
                {
                    AdjClose = stockPrice.Last,
                    Close = stockPrice.Last,
                    Date = stockPrice.Created,
                    High = stockPrice.DaysHigh,
                    Low = stockPrice.DaysLow,
                    Open = stockPrice.Open,
                    Symbol = stockPrice.Stock.Symbol,
                    Volume = 0
                }
                );

            return Ok(result);
        }

        private void PostHistoricalStockPrices(List<HistoricalStockPrice> historicalStockPrices)
        {
            db.HistoricalStockPrices.AddRange(historicalStockPrices);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StockExists(int id)
        {
            return db.Stocks.Count(e => e.ID == id) > 0;
        }
    }
}