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
        public List<Stock> GetStocks()
        {
            List<Stock> result = new List<Stock>();

            List<Stock> stocks = db.Stocks.ToList();

            ApplicationDbContext stockPriceContext = new ApplicationDbContext();

            StockPrice stockPrice = null;

            foreach (var stock in stocks)
            {
                stockPrice = stockPriceContext.StockPrices.Where(s => s.StockID == stock.ID).OrderByDescending(s => s.Created).First();

                if (stockPrice == null || stockPrice.Created < DateTime.Now.AddHours(1))
                {
                    stockPrice = DataFetcher.GetStockQuote(stock.Symbol).StockPrices.FirstOrDefault();
                    stockPrice.StockID = stock.ID;
                    stockPriceContext.StockPrices.Add(stockPrice);
                }

                if (stock.StockPrices.Count > 0)
                {
                    stock.StockPrices.Clear();
                    stock.StockPrices.Add(stockPrice);
                }
                result.Add(stock);
            }

            stockPriceContext.SaveChanges();
            
            return result;
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
                stock = DataFetcher.GetStockQuote(stockSymbol);
                db.Stocks.Add(stock);
                db.SaveChanges();
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