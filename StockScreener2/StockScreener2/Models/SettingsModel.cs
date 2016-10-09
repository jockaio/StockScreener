using StockScreener2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace StockScreener2.Models
{
    public class SettingsModel
    {
        public List<Setting> Settings { get; set; }
    }

    public class Setting
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public CalculationType CalculationType { get; set; }
        public Operator Operator { get; set; }
        public decimal TargetValue { get; set; }
    }
}