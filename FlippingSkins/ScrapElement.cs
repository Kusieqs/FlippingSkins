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

        public ScrapElement(string name) 
        {
            Name = name;
        }
    }

    internal class ScrapRust : ScrapElement
    {
        public float PriceRustSkinsMonkey { get; set; }
        public float PriceRustSteam { get; set; }

        public float PriceRustSkinsWithFee {  get; set; }

        public double Difference {  get; set; }

        public ScrapRust(string name, float priceRustSkinsMonkey) : base (name)
        {
            PriceRustSkinsMonkey = priceRustSkinsMonkey;
        }

        public void SetFeeOnSkinsMonkey()
        {
            float fee = 0.84f;
            PriceRustSkinsWithFee = (float)Math.Round(PriceRustSkinsMonkey * fee,2);
            Difference = Math.Round(PriceRustSkinsWithFee - PriceRustSteam, 2);
        }

    }

    internal class ScrapCSGO : ScrapElement
    {
        public float PriceCSGOSkinsMonkey { get; set; }
        public float PriceCSGOSkinsSteam { get; set; }
        public bool StatTrak { get; set; }
        public string Quality {  get; set; }

        public ScrapCSGO(string name, float priceCSGOSkinsMonkey, bool statTrak, string quality) : base(name)
        {
            PriceCSGOSkinsMonkey = priceCSGOSkinsMonkey;
            StatTrak = statTrak;
            Quality = quality;
        }
    }

}
