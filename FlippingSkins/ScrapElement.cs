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

        public double Procent {  get; set; }

        public ScrapRust(string name, float priceRustSkinsMonkey) : base (name)
        {
            PriceRustSkinsMonkey = priceRustSkinsMonkey;
        }

        public void SetFeeOnSkinsMonkey()
        {
            float fee = 0.84f;
            PriceRustSkinsWithFee = PriceRustSkinsMonkey * fee;
            Procent = (100 * PriceRustSkinsMonkey) / PriceRustSkinsWithFee;
        }

    }

    internal class ScrapCSGO : ScrapElement
    {
        public float PriceCSGOSkinsMonkey { get; set; }
        public float PriceCSGOSkinsSteam { get; set; }
        public ScrapCSGO(string name, float priceCSGOSkinsMonkey) : base(name)
        {
            PriceCSGOSkinsMonkey = priceCSGOSkinsMonkey;
        }
    }

}
