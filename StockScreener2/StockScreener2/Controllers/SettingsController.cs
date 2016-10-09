using Microsoft.AspNet.Identity;
using StockScreener2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;


namespace StockScreener2.Controllers
{
    public class SettingsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Settings
        public List<Setting> GetSettings()
        {
            string userId = User.Identity.GetUserId();
            return db.Settings.Where(s => s.UserID == userId).ToList();
        }

        //POST: api/SaveSetting
        public void SaveSetting(Setting setting)
        {
            db.Settings.Add(setting);
            db.SaveChanges();
        }

        //PUT: api/PutSetting/
        public IHttpActionResult PutStock(int id, Setting setting)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != setting.ID)
            {
                return BadRequest();
            }

            db.Entry(setting).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(id))
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

        private bool SettingExists(int id)
        {
            return db.Settings.Count(e => e.ID == id) > 0;
        }
    }
}