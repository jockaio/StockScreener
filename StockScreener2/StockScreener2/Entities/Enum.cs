using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockScreener2.Entities
{
    public enum CalculationType
    {
        LowSpreadPercentage
    }

    public enum Operator
    {
        Plus,
        Minus,
        MoreThan,
        LessThan,
        EqualTo,
        LessOrEqualTo,
        MoreOrEqualTo
    }
}