using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippingSkins
{
    internal class ScrapElement
    {
        public string Name { get; set; }
        public float PriceRustSkinsMonkey { get; set; }
        public float PriceRustSteam {  get; set; }
        public float PriceCSGOSkinsMonkey { get; set; }
        public float PriceCSGOSkinsSteam { get; set; }
        public ScrapElement(string name, float priceRustSkinsMonkey) 
        {
            Name = name;
            PriceRustSkinsMonkey = priceRustSkinsMonkey;
        }
    }
}
