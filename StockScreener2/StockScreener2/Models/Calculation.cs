using StockScreener2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockScreener2.Models
{
    public class Calculation
    {
        public decimal Value { get; set; }
        public CalculationType CalculationType { get; set; }
    }
}