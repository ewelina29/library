using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Biblioteka.Enums
{
    public enum Status
    {
        [Description("In stock")]
        IN_STOCK,

        [Description("Reserved")]
        RESERVED,

        [Description("Rented")]
        RENTED
    }
}