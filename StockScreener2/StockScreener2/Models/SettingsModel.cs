using StockScreener2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockScreener2.Models
{
    public class SettingsModel
    {
        public List<Setting> Settings { get; set; }
    }

    public class Setting
    {
        public int ID { get; set; }

        public int UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        CalculationType CalculationType { get; set; }
    }
}