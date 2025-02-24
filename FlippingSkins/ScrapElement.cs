using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippingSkins
{
    internal class ScrapElement
    {
        protected const float feeRustSkinsMonkey = 0.84f;
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
        public double ProcentOfPrice {  get; set; }

        public ScrapRust(string name, float priceRustSkinsMonkey) : base (name)
        {
            PriceRustSkinsMonkey = priceRustSkinsMonkey;
        }

        private float PriceAfterFee() => (float)Math.Round(PriceRustSkinsMonkey * feeRustSkinsMonkey, 2);

        private double ProfitOfBuy() => Math.Round(PriceAfterFee() - PriceRustSteam, 2);

        public void SetProcent()
        {
            ProcentOfPrice = Math.Round(PriceAfterFee() * 100 / PriceRustSteam, 2);
        }

        public void Description()
        {
            Console.WriteLine($"Name:                    {Name}");
            Console.WriteLine($"Sell item on SkinsMonkey:{PriceAfterFee()}$");
            Console.WriteLine($"Buy order Steam:         {PriceRustSteam}$");
            Console.WriteLine($"Difference:              {ProfitOfBuy()}$");
            Console.WriteLine($"% of price SkinsMonkey:  {ProcentOfPrice}$");
            Console.WriteLine("\n");
        }
    }

    internal class ScrapCSGO : ScrapElement
    {
        public float PriceCSGOSkinsMonkey { get; set; }
        public float PriceCSGOSkinsSteam { get; set; }
        public float PriceCSGOSkinsWithFee { get; set; }
        public double Difference { get; set; }

        public ScrapCSGO(string name, float priceCSGOSkinsMonkey) : base(name)
        {
            PriceCSGOSkinsMonkey = priceCSGOSkinsMonkey;
        }

        public void SetFeeOnSteam()
        {
            float fee = 0.13f;
            PriceCSGOSkinsWithFee = (float)Math.Round(PriceCSGOSkinsSteam * fee, 2);
            Difference = Math.Round(PriceCSGOSkinsWithFee - PriceCSGOSkinsMonkey, 2);
        }

        public void Description()
        {
            Console.WriteLine("Best deals Steam -> SkinsMoneky:");
            Console.WriteLine($"Name: {Name}\nDifference: {Difference}$\nBuy order Steam: {PriceCSGOSkinsSteam}$\nSell SkinsMoneky {PriceCSGOSkinsMonkey}$\n\n");
        }
    }

}
