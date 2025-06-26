using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippingSkins
{
    public class JsonCSGOPrices
    {
        public bool success { get; set; }
        public string? lowest_price { get; set; }
        public string? volume { get; set; }
        public string? median_price { get; set; }
    }
}
